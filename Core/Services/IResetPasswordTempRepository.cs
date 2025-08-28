using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Identity;

namespace Core.Services
{
    public interface IResetPasswordTempRepository
    {
        Task<ResetPasswordTemp> GetByEmailAsync(string email);
        Task AddAsync(ResetPasswordTemp model);
        Task UpdateAsync(ResetPasswordTemp model);
        Task DeleteAsync(ResetPasswordTemp entity);
    }
}
