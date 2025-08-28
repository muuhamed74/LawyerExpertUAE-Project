using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repo.Configurations;

namespace Repo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurations(new ProductTypeConfiguration())
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<ContractTemplate> ContractTemplates { get; set; }
        public DbSet<UserContract> UserContracts { get; set; }
        public DbSet<UserStoreTemporary> UserStoreTemporary { get; set; }
        public DbSet<ResetPasswordTemp> ResetPasswordTemps { get; set; }
    }
}
