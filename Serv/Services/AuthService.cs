using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.DTOs.Identity;
using Core.Models.Identity;
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Serv.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITemporaryUserRepository _temporery;
        private readonly IEmailSender _emailSender;
        private readonly IResetPasswordTempRepository _resetRepo;

        public AuthService(UserManager<AppUser> userManager,
            IMapper mapper,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager,
            ITemporaryUserRepository temporery,
            IEmailSender emailSender,
            IResetPasswordTempRepository resetRepo)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _temporery = temporery;
            _emailSender = emailSender;
            _resetRepo = resetRepo;
        }


        #region SignUp
        public async Task<AuthModel> InitSignUpAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new AuthModel { Message = "Email is already registered." };


            // Validate password with existing validators
            foreach (var validator in _userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(_userManager, null!, model.Password);
                if (!result.Succeeded)
                {
                    return new AuthModel
                    {
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }
            }



            var existingTempUser = await _temporery.GetByEmailAsync(model.Email);

            var tempUser = _mapper.Map<UserStoreTemporary>(model);

            // generate a random OTP code and add it to the temporary user
            tempUser.Code = GenerateOtpCode();
            tempUser.ExpiresAt = DateTime.UtcNow.AddMinutes(10);


            if (existingTempUser is null)
            {
                await _temporery.AddAsync(tempUser);
            }
            else
            {
                existingTempUser.Email = tempUser.Email;
                existingTempUser.Name = tempUser.Name;
                existingTempUser.Password = tempUser.Password;
                existingTempUser.RePassword = tempUser.RePassword;
                existingTempUser.Phone = tempUser.Phone;
                existingTempUser.Code = tempUser.Code;

                await _temporery.UpdateAsync(existingTempUser);
            }

            // Send Email
            await _emailSender.SendEmailAsync(model.Email, "Your verification code", $"Your code is: {tempUser.Code}");
            return new AuthModel { Email = model.Email, IsAuthenticated = true };

        }


        public async Task<AuthModel> VerifyCodeAsync(string email, string inputCode)
        {
            var existingTempUser = await _temporery.GetByEmailAsync(email);

            if (existingTempUser == null)
                return new AuthModel { Message = "Invalid" };

            if (existingTempUser.Code != inputCode)
                return new AuthModel { Message = "Invalid" };
            ////////////
            var user = _mapper.Map<AppUser>(existingTempUser);

            var result = await _userManager.CreateAsync(user, existingTempUser.Password);

            if (!result.Succeeded)
                return new AuthModel { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            await _userManager.AddToRoleAsync(user, "User");

            var jwtToken = await _tokenService.CreateTokenAsync(user, _userManager);

            await _temporery.DeleteAsync(existingTempUser);

            // Generate a new refresh token and add it to the user
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens = user.RefreshTokens ?? new List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);
            user.RefreshTokenExpiration = refreshToken.ExpiersOn;
            await _userManager.UpdateAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = jwtToken,
                Email = user.Email,
                Name = user.Name,
                Roles = new List<string> { "User" },
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiersOn
            };
        }


        public async Task<SignUpResponseDto> ReSendCode(string email)
        {
            var existingUser = await _temporery.GetByEmailAsync(email);

            if (existingUser == null)
            {
                return new SignUpResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid"
                };
            }

            // Generate new OTP code and update expiration
            var newCode = GenerateOtpCode();
            existingUser.Code = newCode;
            existingUser.ExpiresAt = DateTime.UtcNow.AddMinutes(10);

            await _temporery.UpdateAsync(existingUser);


            await _emailSender.SendEmailAsync(
                existingUser.Email,
                "Your new verification code",
                $"Your verification code is: {newCode}"
            );

            return new SignUpResponseDto
            {
                IsSuccess = true,
                Email = existingUser.Email,
                Message = "Verification code resent successfully."
            };
        }

        #endregion


        #region SignIn
        public async Task<AuthModel> LoginAsync(LoginDto model)
        {
            var authmodel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                authmodel.Message = "Invalid Email or Password";
                return authmodel;
            }
            ;

            var token = await _tokenService.CreateTokenAsync(user, _userManager);
            var roles = await _userManager.GetRolesAsync(user);


            authmodel.Email = model.Email;
            authmodel.IsAuthenticated = true;
            authmodel.Token = token;
            authmodel.Roles = roles.ToList();



            // check if user has an active refresh token if not generate a new one 
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authmodel.RefreshToken = activeToken.Token;
                authmodel.RefreshTokenExpiration = activeToken.ExpiersOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authmodel.RefreshToken = refreshToken.Token;
                authmodel.RefreshTokenExpiration = refreshToken.ExpiersOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }


            return authmodel;

        }
        #endregion


        #region AddRole
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid User ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User Already Has This Role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "Something Went Wrong";

        }
        #endregion



        #region Reset Password
        public async Task<SendOtpCodeResponceDto> SendOtpCode(SendOtpCodeRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new SendOtpCodeResponceDto
                {
                    Success = true,
                    Message = "If this email is registered, a verification code has been sent."
                };
            }


            var code = GenerateOtpCode();
            var expiresAt = DateTime.UtcNow.AddMinutes(10);


            var tempResetModel = new ResetPasswordTemp
            {
                Email = model.Email,
                OtpCode = code,
                ExpiresAt = expiresAt
            };

            await _resetRepo.AddAsync(tempResetModel);


            await _emailSender.SendEmailAsync(model.Email, "Password Reset Code", $"Your reset code is: {code}");

            return new SendOtpCodeResponceDto
            {
                Success = true,
                Message = "A verification code has been sent to your email."
            };
        }


        public async Task<VerifyOtpResponseDto> VerifyResetPasswordOtpAsync(VerifyOtpRequestDto model)
        {
            var tempUser = await _resetRepo.GetByEmailAsync(model.Email);

            if (tempUser == null)
            {
                return new VerifyOtpResponseDto
                {
                    IsVerified = false,
                    Message = "Invalid"
                };
            }

            if (tempUser.OtpCode != model.Code)
            {
                return new VerifyOtpResponseDto
                {
                    IsVerified = false,
                    Message = "Invalid"
                };
            }

            if (tempUser.ExpiresAt < DateTime.UtcNow)
            {
                return new VerifyOtpResponseDto
                {
                    IsVerified = false,
                    Message = "Invalid"
                };
            }

            await _resetRepo.UpdateAsync(tempUser);


            return new VerifyOtpResponseDto
            {
                IsVerified = true,
                Message = "OTP verified successfully. You may now reset your password."
            };
        }


        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto model)
        {
            var tempUser = await _resetRepo.GetByEmailAsync(model.Email);

            if (tempUser == null)
            {
                return new ResetPasswordResponseDto
                {
                    IsReset = false,
                    Message = "Invalid"
                };
            }

            if (tempUser.OtpCode != model.Code || tempUser.ExpiresAt < DateTime.UtcNow)
            {
                return new ResetPasswordResponseDto
                {
                    IsReset = false,
                    Message = "Invalid"
                };
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new ResetPasswordResponseDto
                {
                    IsReset = false,
                    Message = "Passwords do not match."
                };
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ResetPasswordResponseDto
                {
                    IsReset = false,
                    Message = "User not found."
                };
            }

            // Remove old password and add new one
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                return new ResetPasswordResponseDto
                {
                    IsReset = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            await _resetRepo.DeleteAsync(tempUser);

            return new ResetPasswordResponseDto
            {
                IsReset = true,
                Message = "Password has been reset successfully."
            };
        }
        #endregion






        #region Refresh Token
        public async Task<AuthModel> RefreshTokenAsync(string token, string email)
        {
            var authmodel = new AuthModel();
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user is null)
            {
                authmodel.Message = "Invalid Token";
                return authmodel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
            {
                authmodel.Message = "Inactive Token";
                return authmodel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;


            // Generate a new refresh token
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            user.RefreshTokens.Remove(refreshToken);
            await _userManager.UpdateAsync(user);

            // Create a new access token
            var newToken = await _tokenService.CreateTokenAsync(user, _userManager);
            authmodel.IsAuthenticated = true;
            authmodel.Token = newToken;
            authmodel.Email = user.Email;
            authmodel.Name = user.Name;
            var roles = await _userManager.GetRolesAsync(user);
            authmodel.Roles = roles.ToList();
            authmodel.RefreshToken = newRefreshToken.Token;
            authmodel.RefreshTokenExpiration = newRefreshToken.ExpiersOn;
            return authmodel;
        }
        #endregion


        #region Revoke Token
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user is null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
        #endregion


        #region logout
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

            if (user == null)
                return false;

            var token = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

            if (token == null || !token.IsActive)
                return false;

            token.RevokedOn = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        #endregion


















        #region Private Methods
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiersOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        private string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public Task<AuthModel> RefreshTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
