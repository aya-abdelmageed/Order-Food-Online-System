using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        => _dbContext.Set<T>().AddAsync(entity);

        public void Delete(T entity)
        => _dbContext.Set<T>().Remove(entity);

        public void Update(T entity)
        => _dbContext.Set<T>().Update(entity);

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var data = await _dbContext.Set<T>().ToListAsync();
            if(data == null)
               return new List<T>();
            return data;
        }
        //Expression<Func<T, bool>> criteria
        public async Task<T> GetByIdAsync(int id  )
        {
            var data = await _dbContext.Set<T>().FindAsync(id).AsTask();
            //var query=_dbContext.Set<T>().Where(criteria).Include(x => Object).Include(x=>x.Categories).ThenInclude(c=>c.Meals).FirstOrDefault();
            if(data == null)
                return new();

            return data;
        }


    }
}
