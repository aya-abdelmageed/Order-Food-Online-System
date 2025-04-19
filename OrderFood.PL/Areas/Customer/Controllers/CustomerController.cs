using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.PL.Areas.Customer.Models;
using SQLitePCL;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerController : Controller
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        public CustomerController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
        // GET: CustomerController

        //the customer main page
        public async Task<ActionResult> Index(string? name)
        {
            //get the categories data to view
            var categories = await UnitOfWork.GetRepository<Category>().GetAllAsync(includes: m => m.Include(t => t.Meals));
            //get the distinct categories name
            var distinctCategories = categories.GroupBy(c => c.Name).Select(g => new CategoryViewModel()
            {
                CategoryName = g.Key,
                Image = g.First().Image,
            }
            ).ToList();
            //get restarant info to view
            var restaurants = await UnitOfWork.GetRepository<Restaurant>().GetAllAsync(
                includes: i => i.Include(c => c.Categories));

            //
            if (name != null)
            {
                restaurants = restaurants.Where(r => r.Categories.Any(c => c.Name == name)).ToList();
            }
            ViewData["Restaurants"] = restaurants;

            return View(distinctCategories);
        }


        //The Customer OrderHistory Action
        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var order = await UnitOfWork.GetRepository<Order>().GetAllAsync(query => !query.IsDelete && query.CustomerId == "306e5a79-171d-49c5-96c3-3e63953555a7",o => o
                .Include(i => i.OrderMeals)
                .ThenInclude(x => x.Meal)
                );
            return View(order);
        }

    }
}
