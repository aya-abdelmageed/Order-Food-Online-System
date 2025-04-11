using Microsoft.AspNetCore.Mvc;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.RepoTest;

namespace OrderFood.PL.Controllers
{
    public class empController : Controller
    {
        public empController(IGenericRepository<Employee> generic, IEmployeeRepository y )
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
