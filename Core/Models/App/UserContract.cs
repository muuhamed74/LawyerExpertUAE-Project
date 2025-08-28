using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.App;
using Core.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Models
{
    public class UserContract : BaseModel
    {

        
        public int? TemplateId { get; set; }
        public ContractTemplate? Template { get; set; }

      
        public string? UserId { get; set; }        
        

        public string? FilePath { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
