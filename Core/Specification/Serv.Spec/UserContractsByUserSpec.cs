using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.Specification.Main;

namespace Core.Specification.Serv.Spec
{
    public class UserContractsByUserSpec : BaseSpecification<UserContract>
    {
        public UserContractsByUserSpec(string userId)
              : base(uc => uc.UserId == userId)
        {

        }

    }
}
