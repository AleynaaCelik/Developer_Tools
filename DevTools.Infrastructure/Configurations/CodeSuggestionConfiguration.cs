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
    public class CodeSuggestionConfiguration : IEntityTypeConfiguration<CodeSuggestion>
    {
        public void Configure(EntityTypeBuilder<CodeSuggestion> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");

            builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(1000).IsRequired();
            builder.Property(e => e.Category).HasConversion<int>().IsRequired();
            builder.Property(e => e.CodeExample).HasMaxLength(5000);
            builder.Property(e => e.Priority).IsRequired();
            builder.Property(e => e.IsApplied).HasDefaultValue(false);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(e => e.Session)
                   .WithMany(s => s.Suggestions)
                   .HasForeignKey(e => e.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.SessionId);
            builder.HasIndex(e => e.Priority);
            builder.HasIndex(e => e.Category);
        }
    }
}
