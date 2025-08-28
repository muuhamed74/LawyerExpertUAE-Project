using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Models.App;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Services
{
    public interface ISpecification<T>  where T : BaseModel
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includables { get; set; }

        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool enablepagination { get; set; }

    }
}
