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
        {
            //=> _dbContext.Set<T>().Remove(entity);

            entity.IsDelete = true;
            _dbContext.Set<T>().Update(entity);
        }

        public void Update(T entity)
        => _dbContext.Set<T>().Update(entity);


        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null,
                                                        Func<IQueryable<T>, IQueryable<T>>? includes = null,
                                                        Expression<Func<T, object>>? orderBy = null,
                                                        OrderBy orderType = OrderBy.Ascending)
        {
            //    _dbContext.Set<T>()
            //   .Where(r=>r.name == "C")
            //   .Include(R=>R.Owner)
            //   .Include(R=>R.Categories)!
            //   .ThenInclude(Cat=>cat.Meals);
            //   .OrderBy || OrderByDescending (r=>r.location)


            IQueryable<T> data = _dbContext.Set<T>().Where(e => e.IsDelete == false);

            if (criteria != null)
                data = data.Where(criteria);

            if(includes != null)
                data = includes(data);

            if (orderBy != null && orderType == OrderBy.Ascending)
                data = data.OrderBy(orderBy);

            else if (orderBy != null && orderType == OrderBy.Descending)
                data = data.OrderByDescending(orderBy);


            return await data.ToListAsync();
        }
        
        public async Task<T> GetOneAsync(Expression<Func<T, bool>> criteria, Func<IQueryable<T>, IQueryable<T>>? includes = null)
        {
            //var query =

            //     _dbContext.Set<T>()
            //    .Where(criteria)
            //    .Include(includes[0])
            //    .Include(includes[1])!
            //    .ThenInclude();
            //.FirstOrDefault();

            IQueryable<T> query = _dbContext.Set<T>().Where(e => e.IsDelete == false).Where(criteria);

            if (includes != null)
                query = includes(query);
            
            var data = await query.FirstOrDefaultAsync();

            return data == null ? new() : data;
        }


    }
}
