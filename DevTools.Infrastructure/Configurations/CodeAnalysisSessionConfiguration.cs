using DevTools.Core.Entities;
using DevTools.Core.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Infrastructure.Configurations
{
    public class CodeAnalysisSessionConfiguration : IEntityTypeConfiguration<CodeAnalysisSession>
    {
        public void Configure(EntityTypeBuilder<CodeAnalysisSession> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");

            builder.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            builder.Property(e => e.Language).HasConversion<int>().IsRequired();
            builder.Property(e => e.AnalysisType).HasConversion<int>().IsRequired();
            builder.Property(e => e.Status).HasConversion<int>().HasDefaultValue(AnalysisStatus.Pending);
            builder.Property(e => e.OriginalCode).IsRequired();
            builder.Property(e => e.Cost).HasColumnType("decimal(10,4)");
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(e => e.User)
                   .WithMany(u => u.CodeAnalysisSessions)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Project)
                   .WithMany(p => p.AnalysisSessions)
                   .HasForeignKey(e => e.ProjectId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(e => e.Issues)
                   .WithOne(i => i.Session)
                   .HasForeignKey(i => i.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Suggestions)
                   .WithOne(s => s.Session)
                   .HasForeignKey(s => s.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.AnalysisType);
            builder.HasIndex(e => e.ProjectId);
            builder.HasIndex(e => e.Status);
        }
    }
}
