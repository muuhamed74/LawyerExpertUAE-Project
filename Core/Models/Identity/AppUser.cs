using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Core.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }

        public Adress? Adress { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();

        public DateTime? RefreshTokenExpiration { get; set; }

    }
}
