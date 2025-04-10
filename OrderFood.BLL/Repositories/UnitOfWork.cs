using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodDbContext _dbContext;
        private Dictionary<string, object> _repositories;

        public UnitOfWork(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Dictionary<string, object>();
        }

        public IGenericRepository<T> GetRepository<T>() where T : BaseEntity, new()
        {
            string type = typeof(T).Name;
            if (!_repositories.ContainsKey(type))
            {
                GenericRepository<T> repository = new GenericRepository<T>(_dbContext);
                _repositories.Add(type, repository);
            }
            return (IGenericRepository<T>)_repositories[type];
        }

        public ValueTask DisposeAsync()
            => _dbContext.DisposeAsync();

        public async Task SaveChangesAsync()
         => await _dbContext.SaveChangesAsync();
    }
}
