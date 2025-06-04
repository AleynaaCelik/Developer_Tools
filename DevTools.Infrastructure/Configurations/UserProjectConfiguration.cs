using DevTools.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Infrastructure.Configurations
{

    public class UserProjectConfiguration : IEntityTypeConfiguration<UserProject>
    {
        public void Configure(EntityTypeBuilder<UserProject> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");

            builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.Repository).HasMaxLength(500);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(e => e.User)
                   .WithMany(u => u.UserProjects)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.AnalysisSessions)
                   .WithOne(s => s.Project)
                   .HasForeignKey(s => s.ProjectId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
            builder.HasIndex(e => e.IsActive);
        }
    }
}
