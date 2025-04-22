using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Customer.Models;
using SQLitePCL;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerController : Controller
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this.UnitOfWork = unitOfWork;
            _userManager = userManager;
        }
        // GET: CustomerController

        //the customer main page
        public async Task<ActionResult> CustomerHomeAllResturant(string? name)
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

        //The Customer Order List & Details History Action
        [HttpGet]
        public async Task<IActionResult> OrdersListDetails()
        {
            // ex "306e5a79-171d-49c5-96c3-3e63953555a7"
            var user = await _userManager.GetUserAsync(User);
            var order = await UnitOfWork.GetRepository<Order>().GetAllAsync(query => query.CustomerId == user.Id,
                o => o.Include(c => c.Customer)
                .Include(r => r.Restaurant)
                .Include(p => p.Coupon)
                .Include(i => i.OrderMeals)
                .ThenInclude(x => x.Meal)
                );
            return View(order);
        }

        //The Customer Order Filterig Action
        [HttpGet]
        public async Task<IActionResult> OrderFilter(string? searchTerm, string? selectedStatus)
        {
            var user = await _userManager.GetUserAsync(User);
            var query = await UnitOfWork.GetRepository<Order>()
                .GetAllAsync(query => query.CustomerId == user.Id, 
                o => o.Include(c => c.Customer)
                .Include(r => r.Restaurant)
                .Include(p => p.Coupon)
                .Include(i => i.OrderMeals)
                .ThenInclude(x => x.Meal)
                );
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Restaurant.Name.ToLower().Contains(searchTerm.ToLower()));
            }
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                query = query.Where(r => r.OrderStatus.ToString() == selectedStatus);
            }

            return PartialView("_OrderslistPartial", query);
        }





        //----------------------Cusotmer Cart------------------------------


        public async Task<IActionResult> userCart() {

            return View();
        }


        public async Task<IActionResult> userWishlist()
        {

            return View();
        }

        public async Task<IActionResult> userCheckout()
        {

            return View();
        }


        //view meal details

        public async Task<IActionResult> MealDetails(int id)
        {
            var meal =await UnitOfWork.GetRepository<Meal>().GetOneAsync(m => m.Id == id);
            
            return View(meal);
        }
    }
}
