using DevTools.Core.Entities;
using DevTools.Core.Enums;
using DevTools.Core.Interfaces.Repositories;
using DevTools.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Infrastructure.Repositories
{
    public class CodeAnalysisSessionRepository : Repository<CodeAnalysisSession>, ICodeAnalysisSessionRepository
    {
        public CodeAnalysisSessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CodeAnalysisSession>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(s => s.UserId == userId)
                .Include(s => s.Issues)
                .Include(s => s.Suggestions)
                .Include(s => s.Project)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CodeAnalysisSession>> GetByUserIdAndTypeAsync(Guid userId, AnalysisType type)
        {
            return await _dbSet
                .Where(s => s.UserId == userId && s.AnalysisType == type)
                .Include(s => s.Issues)
                .Include(s => s.Suggestions)
                .Include(s => s.Project)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CodeAnalysisSession>> GetRecentSessionsAsync(Guid userId, int count = 10)
        {
            return await _dbSet
                .Where(s => s.UserId == userId)
                .Include(s => s.Issues)
                .Include(s => s.Suggestions)
                .Include(s => s.Project)
                .OrderByDescending(s => s.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCostByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(s => s.UserId == userId)
                .SumAsync(s => s.Cost);
        }

        public async Task<int> GetUsageCountByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(s => s.UserId == userId)
                .CountAsync();
        }

        public async Task<IEnumerable<CodeAnalysisSession>> GetByProjectIdAsync(Guid projectId)
        {
            return await _dbSet
                .Where(s => s.ProjectId == projectId)
                .Include(s => s.Issues)
                .Include(s => s.Suggestions)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
