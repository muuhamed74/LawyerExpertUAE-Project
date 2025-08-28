using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specification.Params
{
    public class ContractTemplateParams
    {
        public String? Sort { get; set; }


        //Default return 3 Templets in page
        private int pagesize = 3;

        public int Pagesize
        {
            // default pagesize initialization
            get { return pagesize; }
            set { pagesize = (value > 0 && value <= 50) ? value : 3; }
        }

        // default return first page
        public int PageIndex { get; set; } = 1;

        public int? Count { get; set; }



        private String? search;

        public String? Search
        {
            get { return search; }
            set { search = value.ToLower(); }
        }
    }
}
