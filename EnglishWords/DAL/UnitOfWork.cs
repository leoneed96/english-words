using EnglishWords.Data;
using EnglishWords.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishWords.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dbContext;
        public UnitOfWork(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            return new Repository<TEntity>(_dbContext);
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        int SaveChanges();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    }
}
