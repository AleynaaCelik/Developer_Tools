using DevTools.Core.Interfaces.Repositories;
using DevTools.Core.Interfaces;
using DevTools.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICodeAnalysisSessionRepository CodeAnalysisSessions =>
            GetRepository<CodeAnalysisSessionRepository>() as ICodeAnalysisSessionRepository ??
            throw new InvalidOperationException();

        public IUserProjectRepository UserProjects =>
            GetRepository<UserProjectRepository>() as IUserProjectRepository ??
            throw new InvalidOperationException();

        public IRepository<T> Repository<T>() where T : class
        {
            return GetRepository<Repository<T>>();
        }

        private T GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = Activator.CreateInstance(type, _context);
                _repositories.Add(type, repositoryInstance!);
            }
            return (T)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
