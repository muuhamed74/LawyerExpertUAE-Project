using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Identity
{
    public class ResetPasswordTemp
    {
        [Key]
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? OtpCode { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
