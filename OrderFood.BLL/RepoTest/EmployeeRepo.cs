using OrderFood.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.RepoTest
{
    internal class EmployeeRepo : IEmployeeRepository
    {

        private readonly FoodDbContext _context;

        public EmployeeRepo(FoodDbContext context) 
        {
            this._context = context;
        }
        public void AddAsync(Employee entity)
        => _context.Add(entity);
        public void Delete(Employee entity)
        => _context.Remove(entity);
        public async Task<IEnumerable<Employee>> GetAllAsync()
        => throw new NotImplementedException();

        public Task<Employee> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public void Update(Employee entity)
        {
            throw new NotImplementedException();
        }
    }
}
