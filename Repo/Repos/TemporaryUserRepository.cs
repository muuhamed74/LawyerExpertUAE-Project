using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Identity;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Repo.Data;

namespace Repo.Repos
{
    public class TemporaryUserRepository : ITemporaryUserRepository
    {
        private readonly ApplicationDbContext _context;

        public TemporaryUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UserStoreTemporary> GetByEmailAsync(string email)
        {
            return await _context.UserStoreTemporary.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(UserStoreTemporary user)
        {
            await _context.UserStoreTemporary.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserStoreTemporary user)
        {
            _context.UserStoreTemporary.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserStoreTemporary user)
        {
            _context.UserStoreTemporary.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
