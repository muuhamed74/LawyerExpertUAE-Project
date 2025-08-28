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
    public class ContractTemplatesWithPagingSpec : BaseSpecification<ContractTemplate>
    {
        public ContractTemplatesWithPagingSpec(ContractTemplateParams contractParams) :
            base(x =>
            (String.IsNullOrEmpty(contractParams.Search) || x.Title.ToLower().Contains(contractParams.Search)))
        {

            ApplyPagination(contractParams.Pagesize * (contractParams.PageIndex - 1), contractParams.Pagesize);



            // for devolepment purpose (if you want to add sorting with price or any thing)


            if (!string.IsNullOrEmpty(contractParams.Sort))
            {
                switch (contractParams.Sort.ToLower())
                {
                    case "titleasc":
                        AddOrderBy(t => t.Title);
                        break;
                    case "titledesc":
                        AddOrderByDesending(t => t.Title);
                        break;
                    default:
                        AddOrderBy(t => t.Id);
                        break;


                }
            }
        }
    }
}
  

