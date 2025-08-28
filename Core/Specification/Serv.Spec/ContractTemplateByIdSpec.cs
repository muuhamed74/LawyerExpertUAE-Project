using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.Specification.Main;

namespace Core.Specification.Serv.Spec
{
    public class ContractTemplateByIdSpec : BaseSpecification<ContractTemplate>
    {
        public ContractTemplateByIdSpec(int id) : base(t => t.Id == id) { }
    }
}
