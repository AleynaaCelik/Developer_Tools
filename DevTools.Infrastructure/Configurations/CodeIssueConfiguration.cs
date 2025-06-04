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
    public class CodeIssueConfiguration : IEntityTypeConfiguration<CodeIssue>
    {
        public void Configure(EntityTypeBuilder<CodeIssue> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");

            builder.Property(e => e.Type).HasConversion<int>().IsRequired();
            builder.Property(e => e.Category).HasConversion<int>().IsRequired();
            builder.Property(e => e.Description).HasMaxLength(1000).IsRequired();
            builder.Property(e => e.Severity).HasConversion<int>().IsRequired();
            builder.Property(e => e.FixSuggestion).HasMaxLength(2000);
            builder.Property(e => e.IsFixed).HasDefaultValue(false);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(e => e.Session)
                   .WithMany(s => s.Issues)
                   .HasForeignKey(e => e.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.SessionId);
            builder.HasIndex(e => e.Severity);
            builder.HasIndex(e => e.Category);
        }
    }
}
