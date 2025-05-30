using DevTools.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICodeAnalysisSessionRepository CodeAnalysisSessions { get; }
        IUserProjectRepository UserProjects { get; }
        IRepository<T> Repository<T>() where T : class;

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
