using OrderFood.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : BaseEntity, new();
        Task SaveChangesAsync();
    }
}
