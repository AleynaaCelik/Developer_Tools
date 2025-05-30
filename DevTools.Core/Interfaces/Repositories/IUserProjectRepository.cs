using DevTools.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Interfaces.Repositories
{
    public interface IUserProjectRepository : IRepository<UserProject>
    {
        Task<IEnumerable<UserProject>> GetByUserIdAsync(Guid userId);
        Task<UserProject?> GetByUserIdAndNameAsync(Guid userId, string name);
        Task<IEnumerable<UserProject>> GetActiveProjectsByUserIdAsync(Guid userId);
    }
}
