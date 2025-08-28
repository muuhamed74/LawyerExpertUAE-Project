using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repo.Configurations
{
    public class ContractTemplateConfiguration : IEntityTypeConfiguration<ContractTemplate>
    {
        public void Configure(EntityTypeBuilder<ContractTemplate> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.FileUrl)
                   .HasMaxLength(500);
        }
    }
}
