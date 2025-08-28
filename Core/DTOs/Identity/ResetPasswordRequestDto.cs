using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Identity
{
    public class ResetPasswordRequestDto
    {
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? NewPassword { get; set; }

        [Required]
        public string? ConfirmPassword { get; set; }

        [Required]
        public string? Code { get; set; }
    }
}
