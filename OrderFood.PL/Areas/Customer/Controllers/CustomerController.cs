using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.Repositories;
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

        //-----------------------------------------------------------------------
        public async Task<ActionResult> CustomerHomeAllResturant(string? name, string? address, string? categoryName)
        {
            var categories = await UnitOfWork.GetRepository<Category>().GetAllAsync(includes: m => m.Include(t => t.Meals));
            var distinctCategories = categories.GroupBy(c => c.Name).Select(g => new CategoryViewModel()
            {
                CategoryName = g.Key,
                Image = g.First().Image,
            }).ToList();

            var restaurants = await UnitOfWork.GetRepository<Restaurant>().GetAllAsync(
                includes: i => i.Include(c => c.Categories));

            if (!string.IsNullOrEmpty(categoryName))
            {
                restaurants = restaurants.Where(r => r.Categories.Any(c => c.Name == categoryName)).ToList();
            }

            if (!string.IsNullOrEmpty(name))
            {
                restaurants = restaurants.Where(r => r.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(address))
            {
                restaurants = restaurants.Where(r => r.Address.ToLower().Contains(address.ToLower())).ToList();
            }

            ViewData["Restaurants"] = restaurants;
            return View(distinctCategories);
        }

        //-----------------------------------------------------------------------
        public async Task<IActionResult> GetMenu(int id)
        {

            var foodDbContext = await UnitOfWork.GetRepository<Restaurant>().GetOneAsync(i => i.Id == id, query => query.Include(p => p.Categories).ThenInclude(m => m.Meals));

            //get the restaurant reviews
            var reviews = await UnitOfWork.GetRepository<Review>().GetAllAsync(i => i.RestaurantId == id && i.IsDelete == false, i => i.Include(r => r.Customer)
            );

            ViewData["Reviews"] = reviews;

            if (foodDbContext == null)
                return NotFound();
            else
                return View(foodDbContext);
        }


        public async Task<IActionResult> GetAllCat(int id)
        {
            var restaurant = await UnitOfWork.GetRepository<Restaurant>()
       .GetOneAsync(r => r.Id == id, query => query
           .Include(r => r.Categories)
           .ThenInclude(c => c.Meals));

            if (restaurant == null)
                return NotFound();

            return View(restaurant);
        }


        public async Task<IActionResult> GetCategoryMeals(int restaurantId, int categoryId)
        {
            var restaurant = await UnitOfWork.GetRepository<Restaurant>()
                .GetOneAsync(r => r.Id == restaurantId, query => query
                .Include(r => r.Categories)
                .ThenInclude(c => c.Meals));

            if (restaurant == null)
                return NotFound();

            var category = restaurant.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
                return NotFound("Category not found in this restaurant.");

            return PartialView("_MealsPartial", category);
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


        public async Task<IActionResult> userCart()
        {

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
            var meal = await UnitOfWork.GetRepository<Meal>().GetOneAsync(m => m.Id == id);

            return View(meal);
        }


        [HttpGet]
        public async Task<IActionResult> FilterRestaurantsByCategory(string categoryName)
        {
            var restaurants = await UnitOfWork.GetRepository<Restaurant>().GetAllAsync(
                includes: r => r.Include(c => c.Categories));

            if (!string.IsNullOrEmpty(categoryName))
            {
                restaurants = restaurants
                    .Where(r => r.Categories.Any(c => c.Name == categoryName))
                    .ToList();
            }

            return PartialView("_RestaurantCards", restaurants);
        }
        //---------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> FilterRestaurants(string? name, string? address, string? categoryName)
        {
            var restaurants = await UnitOfWork.GetRepository<Restaurant>().GetAllAsync(
                includes: r => r.Include(c => c.Categories));

            if (!string.IsNullOrEmpty(categoryName))
            {
                restaurants = restaurants.Where(r => r.Categories.Any(c => c.Name == categoryName)).ToList();
            }

            if (!string.IsNullOrEmpty(name))
            {
                restaurants = restaurants.Where(r => r.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(address))
            {
                restaurants = restaurants.Where(r => r.Address.ToLower().Contains(address.ToLower())).ToList();
            }

            return PartialView("_RestaurantCards", restaurants);
        }
        //-----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> SearchMeals(int restaurantId, string searchTerm = "", int? categoryId = null, decimal? maxPrice = null)
        {
            var restaurant = await UnitOfWork.GetRepository<Restaurant>()
                .GetOneAsync(r => r.Id == restaurantId,
                query => query.Include(r => r.Categories)
                              .ThenInclude(c => c.Meals));

            if (restaurant == null)
                return NotFound();

            // Filter meals based on search criteria
            var meals = restaurant.Categories
                .SelectMany(c => c.Meals)
                .Where(m => !m.IsDelete);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Case-insensitive "starts with" search
                meals = meals.Where(m => m.Name.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (maxPrice.HasValue)
            {
                meals = meals.Where(m => m.Price <= maxPrice.Value);
            }

            if (categoryId.HasValue)
            {
                meals = meals.Where(m => m.CategoryId == categoryId.Value);
            }

            return PartialView("_MealsPartial", new Category
            {
                Meals = meals.ToList(),
                Name = "Search Results"
            });
        }

    }
}