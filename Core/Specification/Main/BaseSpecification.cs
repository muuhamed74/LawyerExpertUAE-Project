using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Models.App;
using Core.Services;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Specification.Main
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseModel
    {
        public Expression<Func<T, bool>> Criteria { get; set; }

        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();

        public List<string> IncludeStrings { get; set; }

        public Expression<Func<T, object>> OrderBy { get; set; }

        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool enablepagination { get; set; }
        public List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includables { get; set; } = new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>>();


        public BaseSpecification()
        {

        }



        public BaseSpecification(Expression<Func<T, bool>> CriteriaExpression)
        {
            Criteria = CriteriaExpression;
        }


        public void AddOrderBy(Expression<Func<T, object>> OrderByexpression)
        {
            OrderBy = OrderByexpression;
        }

        public void AddOrderByDesending(Expression<Func<T, object>> OrderByDesexpression)
        {
            OrderByDescending = OrderByDesexpression;
        }

        public void ApplyPagination(int skip, int take)
        {
            enablepagination = true;
            Skip = skip;
            Take = take;
        }

        public void AddInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeExpression)
        {
            Includables.Add(includeExpression);
        }

    }
}
