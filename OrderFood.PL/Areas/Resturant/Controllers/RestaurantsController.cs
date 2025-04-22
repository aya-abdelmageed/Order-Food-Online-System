using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.Repositories;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Delivery.ViewModel;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Resturant.ViewModel;

namespace OrderFood.PL.Areas.Resturant.Controllers
{
    [Area("Resturant")]
    public class RestaurantsController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(IUnitOfWork context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //----------------------------------------------------------------
        // GET: Resturant/Restaurants
        public async Task<IActionResult> GetMenu(int id)
        {

            var foodDbContext = await _context.GetRepository<Restaurant>().GetOneAsync(i => i.Id == id, query => query.Include(p => p.Categories).ThenInclude(m => m.Meals));

            if (foodDbContext == null)
                return NotFound();
            else
                return View (foodDbContext);
        }

        //----------------------------------------------------------------
        public async Task<IActionResult> GetAllCat(int id)
        {
            var restaurant = await _context.GetRepository<Restaurant>()
       .GetOneAsync(r => r.Id == id, query => query
           .Include(r => r.Categories)
           .ThenInclude(c => c.Meals));

            if (restaurant == null)
                return NotFound();

            return View(restaurant);
        }
        //----------------------------------------------------------------------------
        public async Task<IActionResult> GetCategoryMeals(int restaurantId, int categoryId)
        {
            var restaurant = await _context.GetRepository<Restaurant>()
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
        //-------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        { 
            var categoryRepo = _context.GetRepository<Category>();
            var category = await categoryRepo.GetOneAsync(c => c.Id == id, query => query.Include(c => c.Meals));

            if (category == null)
            {
                return Json(new { success = false, message = "Category not found." });
            }

            // Soft delete the category
            category.IsDelete = true;

            // Soft delete its meals
            if (category.Meals != null)
            {
                foreach (var meal in category.Meals)
                {
                    meal.IsDelete = true;
                    _context.GetRepository<Meal>().Update(meal);
                }
            }

            categoryRepo.Update(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Category deleted successfully." });
        }

        //-----------------------------------------------------------------------
        // GET: Add Category
        [HttpGet]
        public IActionResult AddCategory(int id)
        {
            return View(new CategoryViewModel { RestaurantId = id });


        }

        // POST: Add Category
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RestaurantId = model.RestaurantId;
                return View(model);
            }

            string imagePath = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Restaurant");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imagePath = "images/Restaurant/" + fileName;
            }

            var category = new Category
            {
                Name = model.Name,
                RestaurantId = model.RestaurantId,
                IsDelete = false,
                Image = imagePath
            };

             _context.GetRepository<Category>().AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetMenu),new {id= model.RestaurantId});
        }



        //------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> SoftDeleteMeal(int mealId)
        {
            var meal = await _context.GetRepository<Meal>().GetOneAsync(r => r.Id == mealId, query => query
                .Include(r => r.Category)
                .ThenInclude(c => c.Meals));

            if (meal == null)
                return NotFound();

            meal.IsDelete = true;

            _context.GetRepository<Meal>().Update(meal);
            await _context.SaveChangesAsync();

            int restaurantId = meal.Category.RestaurantId;

            return RedirectToAction(nameof(GetAllCat), new { id = restaurantId }); 
        }

        //----------------------------------------------------------------
        //get
        public async Task<IActionResult> UpdateMeal(int id)
        {
            var meal = await _context.GetRepository<Meal>()
                .GetOneAsync(m => m.Id == id);

            if (meal == null)
                return NotFound();

            var categories = await _context.GetRepository<Category>().GetAllAsync();
            ViewBag.allCategories = categories.Where(i => i.RestaurantId ==meal.Category.RestaurantId);
            ViewBag.RestaurantId = meal.Category.RestaurantId;

            return View(meal);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMeal(Meal updatedMeal, int RestaurantId)
        {
            var imageFile = Request.Form.Files["avatar"];
            var existingImagePath = Request.Form["existingImage"];

            if (string.IsNullOrEmpty(updatedMeal.Image) && string.IsNullOrEmpty(existingImagePath) && (imageFile == null || imageFile.Length == 0))
            {
                ModelState.AddModelError("Image", "Image is required.");
            }
            else
            {
                ModelState.Remove("Image");
            }

            if (!ModelState.IsValid)
            {
                var categoryRepo = _context.GetRepository<Category>();
                var selectedCategory = await categoryRepo.GetOneAsync(c => c.Id == updatedMeal.CategoryId);

                if (selectedCategory != null)
                {
                    ViewBag.allCategories = await categoryRepo
                        .GetAllAsync(c => c.RestaurantId == selectedCategory.RestaurantId);
                    ViewBag.RestaurantId = selectedCategory.RestaurantId;
                }

                updatedMeal.Image = existingImagePath;
                return View(updatedMeal);
            }

            var mealRepo = _context.GetRepository<Meal>();
            var existingMeal = await mealRepo.GetOneAsync(m => m.Id == updatedMeal.Id);
            if (existingMeal == null)
                return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/meals");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                existingMeal.Image = "/images/meals/" + fileName;
            }
            else
            {
                existingMeal.Image = existingImagePath;
            }

            existingMeal.Name = updatedMeal.Name;
            existingMeal.Price = updatedMeal.Price;
            existingMeal.Description = updatedMeal.Description;
            existingMeal.CategoryId = updatedMeal.CategoryId;

            mealRepo.Update(existingMeal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetAllCat), new { id = RestaurantId });
        }

        //----------------------------------------------------------------------------
        //GET
        [HttpGet]
        public async Task<IActionResult> AddMeal(int id)
        {
            ViewBag.allCategories = await _context.GetRepository<Category>()
                                               .GetAllAsync(c => !c.IsDelete && c.RestaurantId == id);

            ViewBag.RestaurantId = id;

            return View(new MealViewModel { restaurantId = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddMeal(MealViewModel mealVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.allCategories = await _context.GetRepository<Category>()
                    .GetAllAsync(c => c.RestaurantId == mealVM.restaurantId);
                ViewBag.RestaurantId = mealVM.restaurantId;
                return View(mealVM);
            }

            var meal = new Meal
            {
                Name = mealVM.Name,
                Description = mealVM.Description,
                Price = mealVM.Price,
                CategoryId = mealVM.CategoryId
            };

            // Handle image file
            if (mealVM.ImageFile != null && mealVM.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/meals");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(mealVM.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mealVM.ImageFile.CopyToAsync(stream);
                }

                meal.Image = "/images/meals/" + fileName;
            }

             _context.GetRepository<Meal>().AddAsync(meal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetAllCat), new { id = mealVM.restaurantId });
        }
        //-------------------------------------------------------------------------------------------
        public async Task<IActionResult> SearchMeals(int restaurantId, string? searchTerm, int? categoryId, decimal? maxPrice, int PageNo = 1)
        {
            var restaurant = await _context.GetRepository<Restaurant>().GetOneAsync(i => i.Id == restaurantId, query => query.Include(p => p.Categories).ThenInclude(m => m.Meals));


            if (restaurant == null) return NotFound();

            var meals = restaurant.Categories
                .Where(c => !c.IsDelete && (!categoryId.HasValue || c.Id == categoryId.Value))
                .SelectMany(c => c.Meals)
                .Where(m => !m.IsDelete &&
                    (string.IsNullOrEmpty(searchTerm) || m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || m.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                    (!maxPrice.HasValue || m.Price <= maxPrice.Value))
                .OrderBy(m => m.Price)
                .ToList();

            /* Pagination */
            int NoOfRecordsPerPage = 2;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(meals.Count) / Convert.ToDouble(NoOfRecordsPerPage)));

            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;

            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;

            ViewBag.RestaurantId = restaurantId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.maxPrice = maxPrice;

            meals = meals.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return PartialView("View", meals);
        }
        //----------------------------------------------------------------------------------------
        //Get restaurant reviews
        public async Task<IActionResult> GetReviews(int restaurantID)
        {
            var reviews = await _context.GetRepository<Review>()
                .GetAllAsync(r => r.RestaurantId == 3, i => i.Include(o => o.Restaurant).Include(c => c.Customer));
            return (View(reviews));
        }

        [HttpGet]
        //Get the Restaurant info to Edit
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            var restuarant = await _context.GetRepository<Restaurant>().GetOneAsync(
                criteria: c => c.OwnerId == user.Id
                );
            var model = new UpdateRestaurantViewModel
            {
                Restaurant = restuarant,
                Name = restuarant.Name,
                Address = restuarant.Address,
                Description = restuarant.Description,
                HotLine = restuarant.HotLine,
                Long = restuarant.Long,
                Lat = restuarant.Lat,
                ImageFile = null // Initialize to null
            };
            return (View(model));
        }

        [HttpPost]
        //update restaurant info including uploade files for logo
        public async Task<IActionResult> Settings(UpdateRestaurantViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            //check if the image file is included
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // Ensure wwwroot/images exists
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/restaurant");
                Directory.CreateDirectory(uploadsFolder); // Creates it if not exists

                // Unique file name (to prevent collisions)
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                //copy the path of created file into the Logo field to save to database
                model.Restaurant.Logo = "/images/restaurant/" + uniqueFileName;
            }

            //check the validation restaurant info that included into the model
            if (ModelState.IsValid)
            {
                _context.GetRepository<Restaurant>().Update(model.Restaurant);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Settings");
            }
            else
            {
                return View(model);

            }
        }

        //***************************************************************************************************************
        //get all restaurant orders
        public async Task<IActionResult> GetRestOrders()
        {
            var Ownerid = _UserManager.GetUserId(User);

            var rest = await _context.GetRepository<Restaurant>().GetOneAsync(r => r.OwnerId == Ownerid, q => q.Include(o => o.Orders)!.ThenInclude(o => o.OrderMeals)!.ThenInclude(m => m.Meal));
            if (rest == null)
                return NotFound();
            var restOrders = rest.Orders!.ToList();

            return View(restOrders);
        }


    }
}