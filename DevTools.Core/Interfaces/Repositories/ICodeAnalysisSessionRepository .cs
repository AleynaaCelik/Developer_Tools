using DevTools.Core.Entities;
using DevTools.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Interfaces.Repositories
{
    public interface ICodeAnalysisSessionRepository : IRepository<CodeAnalysisSession>
    {
        Task<IEnumerable<CodeAnalysisSession>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<CodeAnalysisSession>> GetByUserIdAndTypeAsync(Guid userId, AnalysisType type);
        Task<IEnumerable<CodeAnalysisSession>> GetRecentSessionsAsync(Guid userId, int count = 10);
        Task<decimal> GetTotalCostByUserAsync(Guid userId);
        Task<int> GetUsageCountByUserAsync(Guid userId);
        Task<IEnumerable<CodeAnalysisSession>> GetByProjectIdAsync(Guid projectId);
    }
}
