using OrderFood.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Interfaces
{
    public interface IGenericRepository <T> where T : BaseEntity
    {
        void AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity);
        Task<T> GetOneAsync(Expression<Func<T, bool>> criteria, Func<IQueryable<T>, IQueryable<T>>? includes = null);
        
        
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? criteria =null, 
            Func<IQueryable<T>, IQueryable<T>>? includes = null, 
            Expression<Func<T, object>>? orderBy = null,
            OrderBy orderType = OrderBy.Ascending
            );

        //Expression<Func<T, bool>> criteria,
        //int? skip, int? take,
        //Expression<Func<T, object>> orderBy = null,
        //OrderBy orderByDirection, string[] includes = null


    }
}
