using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Identity
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Display name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }
        ///^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$/
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$/",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, one special character, and be at least 6 characters long.")]
        public string? Password { get; set; }


        [Required(ErrorMessage = "Re-entering the password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? RePassword { get; set; }
    }
}
