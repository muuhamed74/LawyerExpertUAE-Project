using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Identity
{
    public class UserStoreTemporary
    {
        [Required, MaxLength(80)]
        public string? Name { get; set; }

        [Key]
        [Required, MaxLength(128)]
        public string? Email { get; set; }

        [Required]
        public string? Phone { get; set; }

        [Required, MaxLength(256)]
        public string? Password { get; set; }

        [Required, MaxLength(256)]
        public string? RePassword { get; set; }
        [Required, MaxLength(10)]
        public string? Code { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
