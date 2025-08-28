using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.Specification.Main;
using Core.Specification.Params;

namespace Core.Specification.Serv.Spec
{
    public class ContractTemplatesWithCountSpec : BaseSpecification<ContractTemplate>
    {
        public ContractTemplatesWithCountSpec(ContractTemplateParams contractParams) :
            base(x =>
            (String.IsNullOrEmpty(contractParams.Search) || x.Title.ToLower().Contains(contractParams.Search)))
        { 
        }
        
    }
        
}
