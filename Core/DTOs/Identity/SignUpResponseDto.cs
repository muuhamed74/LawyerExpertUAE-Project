using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Identity
{
    public class SignUpResponseDto
    {
        public string? Email { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}

