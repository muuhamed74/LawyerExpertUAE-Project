using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repo.Configurations
{
    public class UserContractConfiguration : IEntityTypeConfiguration<UserContract>
    {
        public void Configure(EntityTypeBuilder<UserContract> builder)
        {
            builder.HasKey(uc => uc.Id);

            
            builder.HasOne(uc => uc.Template)
                   .WithMany(t => t.UserContracts)
                   .HasForeignKey(uc => uc.TemplateId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
