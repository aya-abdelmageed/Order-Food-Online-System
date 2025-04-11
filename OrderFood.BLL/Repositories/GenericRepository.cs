using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.RepoTest;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
    {
        private readonly FoodDbContext _dbContext;

        public GenericRepository(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void AddAsync(T entity)
        => _dbContext.AddAsync(entity);

        public void Delete(T entity)
        => _dbContext.Remove(entity);

        public void Update(T entity)
        => _dbContext.Update(entity);

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var data = await _dbContext.Set<T>().ToListAsync();
            if(data == null)
               return new List<T>();
            return data;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var data = await _dbContext.Set<T>().FindAsync(id).AsTask();

            if(data == null)
                return new();

            return data;
        }

    }
}
