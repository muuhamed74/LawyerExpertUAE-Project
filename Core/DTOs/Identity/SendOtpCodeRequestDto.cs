using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Identity
{
    public class SendOtpCodeRequestDto
    {
        [Required, EmailAddress]
        public string? Email { get; set; }
    }
}
