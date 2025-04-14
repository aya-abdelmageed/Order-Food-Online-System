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
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();

        //Expression<Func<T, bool>> criteria, int? skip, int? take,
        //Expression<Func<T, object>> orderBy = null, OrderBy orderByDirection, string[] includes = null


    }
}
