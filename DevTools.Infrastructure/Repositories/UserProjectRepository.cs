using DevTools.Core.Entities;
using DevTools.Core.Interfaces.Repositories;
using DevTools.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Infrastructure.Repositories
{
    public class UserProjectRepository : Repository<UserProject>, IUserProjectRepository
    {
        public UserProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserProject>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .Include(p => p.AnalysisSessions)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<UserProject?> GetByUserIdAndNameAsync(Guid userId, string name)
        {
            return await _dbSet
                .Include(p => p.AnalysisSessions)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Name == name);
        }

        public async Task<IEnumerable<UserProject>> GetActiveProjectsByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId && p.IsActive)
                .Include(p => p.AnalysisSessions)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
