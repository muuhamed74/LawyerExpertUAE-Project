using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Models.App;

namespace Core.Services
{
    public interface IGenericRepository<T> where T : BaseModel
    {

        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdWithSpecAsync(ISpecification<T> spec);

        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        //Task<IReadOnlyList<T>> ListAllAsync();
        //Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);



        Task SaveAsync();
    }
}
