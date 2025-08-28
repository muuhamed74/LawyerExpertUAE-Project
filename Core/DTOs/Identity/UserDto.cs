using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Identity
{
    public class UserDto
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
