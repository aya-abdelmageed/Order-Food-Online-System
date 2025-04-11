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
            //IGenericRepository<T> repo = new GenericRepository<T>(_dbContext);
            //return repo;
            ////IGenericRepository<Employee>

            //"Employee"
            string type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                IGenericRepository<T> repository = new GenericRepository<T>(_dbContext);
                _repositories.Add(type, repository);
            }
            return (IGenericRepository<T>)_repositories[type];
        }

        public ValueTask DisposeAsync()
            => _dbContext.DisposeAsync();

        public async Task SaveChangesAsync()
         => await _dbContext.SaveChangesAsync();
    }



    class anyController
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IGenericRepository<Department> dept;
        //private readonly IGenericRepository<Employee> emp;

        public anyController(IUnitOfWork unitOfWork 
            /*, IGenericRepository<Employee> emp, IGenericRepository<Department> dept*/)
        {
            _unitOfWork = unitOfWork;
            //this.dept = dept;
            //this.emp = emp;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            //var employees = await emp.GetAllAsync();

            var employees = await _unitOfWork.GetRepository<Employee>().GetAllAsync();

            return employees;
        }

        // GET emps with dept
        public async Task<IEnumerable<Employee>> GetAllEmployeesWithDept()
        {
            var employees = await _unitOfWork.GetRepository<Employee>().GetAllAsync();

            var depts = await _unitOfWork.GetRepository<Department>().GetAllAsync();
            var result = from e in employees
                         join d in depts on e.Id equals d.Id
                         select new Employee
                         {
                             Id = e.Id,

                         };
            return result;
        }

        public async Task<Employee> GetEmployeesIdWithDept(int id)
        {
            // issue => Two Objects of _unitOfWork.GetRepository<Employee>()

            // => Key: "Employee" , value: object => IGenericRepository<T>()

            var employee = await _unitOfWork.GetRepository<Employee>().GetByIdAsync(id);
            _unitOfWork.GetRepository<Employee>().Delete(employee);

            await _unitOfWork.SaveChangesAsync();

            var depts = await _unitOfWork.GetRepository<Department>().GetAllAsync();

            return new Employee();

        }
    }


}
