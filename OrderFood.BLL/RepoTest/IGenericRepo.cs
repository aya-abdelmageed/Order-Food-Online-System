using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.RepoTest
{
    internal interface IGenericRepo<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        void AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
