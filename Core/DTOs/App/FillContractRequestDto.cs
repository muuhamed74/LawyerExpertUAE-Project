using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.App
{
    public class FillContractRequestDto
    {
        public int TemplateId { get; set; }
        public string? Data { get; set; }
    }
}
