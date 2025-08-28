using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.App;

namespace Core.Models
{
    public class ContractTemplate : BaseModel
    {              
        public string? Title { get; set; }            
        public string? FileUrl { get; set; }      

        
        public ICollection<UserContract>? UserContracts { get; set; }

    }
}
