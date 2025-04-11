using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.RepoTest
{
    internal interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> GetByIdAsync(int id);
        void AddAsync(Department entity);
        void Delete(Department entity);
        void Update(Department entity);
    }
}
