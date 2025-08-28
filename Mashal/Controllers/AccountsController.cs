using AutoMapper;
using Core.DTOs.Identity;
using Core.Models.Identity;
using Core.Services;
using Mashal.Helpers.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace lawyer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IAuthService _iidentityservice;

        public AccountsController(UserManager<AppUser> userManager,
            ITokenService tokenService,
            SignInManager<AppUser> signInManager,
            IMapper mapper,
            IEmailSender emailSender,
            IAuthService iidentityservice)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _iidentityservice = iidentityservice;
        }



        //Initial SignUp
        [HttpPost("SignUp")]
        // Data will be with json format 
        public async Task<ActionResult> InitSignUpAsync([FromBody] RegisterDto model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _iidentityservice.InitSignUpAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(new ApiResponse(400, result.Message));

            return Ok(result);

            #region old code
            //if (await _userManager.FindByEmailAsync(dto.Email) != null)
            //    return BadRequest(new ApiResponce(400, "This Email Is Alredy Exist"));

            //if (CheckEmailExists(dto.Email).Result.Value)
            //    return BadRequest(new ApiResponce(400, "This Email Is Alredy Exist"));

            //var user = new Appuser
            //{
            //    DisplayName = dto.DisplayName,
            //    Email = dto.Email,
            //    UserName = dto.UserName
            //};

            //var result = await _userManager.CreateAsync(user, dto.Password);

            //if (!result.Succeeded)
            //    return BadRequest(result.Errors);

            //var token = await _tokenService.CreateTokenAsync(user, _userManager);

            //return new UserDto
            //{
            //    DisplayName = user.DisplayName,
            //    Email = user.Email,
            //    Token = token
            //};
            #endregion
        }

        //Initial SignUp Code verification
        [HttpPost("signup/verify")]
        public async Task<IActionResult> VerifySignUpCode([FromQuery] string email, [FromQuery] string code)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _iidentityservice.VerifyCodeAsync(email, code);

            if (!result.IsAuthenticated)
                return BadRequest(new ApiResponse(400, result.Message));

            return Ok(result);
        }


        //Resend OTP
        [HttpPost("signup/resend")]
        public async Task<IActionResult> ReSendCode([FromQuery] string email)
        {
            var result = await _iidentityservice.ReSendCode(email);

            if (!result.IsSuccess)
                return BadRequest(new ApiResponse(400, "Email not found"));

            return Ok(result);
        }



        //Login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _iidentityservice.LoginAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(new ApiResponse(400, result.Message));

            // Set the access token in cookies for secure access
            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);


            #region Old Code
            //    var user = await _userManager.FindByEmailAsync(loginDto.Email);

            //    if (user == null)
            //        return Unauthorized(new ApiResponce(401));

            //    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            //    if (!result.Succeeded)
            //        return Unauthorized(new ApiResponce(401));

            //    return new UserDto
            //    {
            //        DisplayName = user.DisplayName,
            //        Email = user.Email,
            //        Token = await _tokenService.CreateTokenAsync(user, _userManager)
            //    };
            //}
            #endregion
        }


        //Send OTP to reset password
        [HttpPost("password/send-otp")]
        [EnableRateLimiting("OtpPolicy")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> SendOtpCode([FromBody] SendOtpCodeRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _iidentityservice.SendOtpCode(model);

            if (!result.Success)
                return BadRequest(new ApiResponse(400, result.Message));

            return Ok(result);
        }



        //Verify OTP for password reset
        [HttpPost("password/verify-otp")]
        public async Task<IActionResult> VerifyResetPasswordOtp([FromBody] VerifyOtpRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _iidentityservice.VerifyResetPasswordOtpAsync(model);

            if (!result.IsVerified)
                return BadRequest(new ApiResponse(400, result.Message));

            return Ok(result);
        }


        //Reset Password
        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _iidentityservice.ResetPasswordAsync(model);

            if (!result.IsReset)
                return BadRequest(new ApiResponse(400, result.Message));

            return Ok(result);
        }



        //Add Role
        [Authorize(Roles = "Admin")]
        [HttpPost("roles/add")]
        [HttpPost("addrole")]
        public async Task<ActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _iidentityservice.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(new ApiResponse(400, result));

            return Ok(model);
        }



        //Refresh Token
        [HttpGet("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _iidentityservice.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return Unauthorized(new ApiResponse(401, result.Message));

            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }



        //Revoke Token
        [HttpPost("revoke-token")]
        public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new ApiResponse(400, "Token is required"));

            var result = await _iidentityservice.RevokeTokenAsync(token);

            if (!result)
                return BadRequest(new ApiResponse(400, "Token is invalid"));

            return Ok("Token revoked successfully");
        }



        //Logout(invalidate refresh token)
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromQuery] string refreshToken)
        {
            var result = await _iidentityservice.LogoutAsync(refreshToken);

            return result ? Ok("Logged out successfully.") : BadRequest(new ApiResponse(401));
        }








        


        // Refresh Token in Cookie
        private void SetRefreshTokenCookie(string refreshtoken, DateTime expiers)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiers.ToLocalTime(),
            };
            Response.Cookies.Append("refreshToken", refreshtoken, cookieOptions);
        }
    }
}
