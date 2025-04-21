using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.Repositories;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities.Models;
using OrderFood.PL.Areas.Resturant.ViewModel;

namespace OrderFood.PL.Areas.Resturant.Controllers
{
    [Area("Resturant")]
    public class RestaurantsController : Controller
    {
        private readonly IUnitOfWork _context;

        public RestaurantsController(IUnitOfWork context)
        {
            _context = context;
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
        public IActionResult AddCategory(int restaurantId)
        {
            var model = new CategoryViewModel
            {
                RestaurantId = restaurantId
            };
            ViewBag.RestaurantId = restaurantId;
            return View(model);
        }

        // POST: Add Category
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel model, int restaurantId)
        {

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
                RestaurantId = restaurantId,
                IsDelete = false,
                Image = imagePath
            };
            if (!ModelState.IsValid)
            {
                ViewBag.RestaurantId = restaurantId;
                return View(model);
            }

            _context.GetRepository<Category>().AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetMenu), new { id = model.RestaurantId });
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

            return RedirectToAction(nameof(GetAllCat)); 
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

            return View(meal);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMeal(Meal updatedMeal)
        {
            var imageFile = Request.Form.Files["avatar"];
            var existingImagePath = Request.Form["existingImage"];

            // Remove model error for Image if user didn't upload a new file and existing one is there
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
                }

                updatedMeal.Image = existingImagePath;
                return View(updatedMeal);
            }

            var mealRepo = _context.GetRepository<Meal>();
            var existingMeal = await mealRepo.GetOneAsync(m => m.Id == updatedMeal.Id);
            if (existingMeal == null)
                return NotFound();

            //  Handle image upload or use existing
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/meals");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

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

            return RedirectToAction(nameof(GetAllCat));
        }
        //----------------------------------------------------------------------------
        //GET
        [HttpGet]
        public async Task<IActionResult> AddMeal(int restaurantId)
        {
            ViewBag.allCategories = await _context.GetRepository<Category>()
                                               .GetAllAsync(c => !c.IsDelete && c.RestaurantId == restaurantId);

            ViewBag.RestaurantId = restaurantId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMeal(MealViewModel mealVM, int restaurantId)
        {
            var meal = new Meal();

            // Handle image file
            if (mealVM.ImageFile != null && mealVM.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/meals");
                Directory.CreateDirectory(uploadsFolder); 

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(mealVM.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mealVM.ImageFile.CopyToAsync(stream);
                }

                meal.Name = mealVM.Name;
                meal.Description = mealVM.Description;
                meal.Price = mealVM.Price;
                meal.CategoryId = mealVM.CategoryId;
                meal.Image = "/images/meals/" + fileName; 
               
            }
            
            if (!ModelState.IsValid )
            {
                ViewBag.allCategories = await _context.GetRepository<Category>()
                                          .GetAllAsync(c => c.RestaurantId == restaurantId);

                ViewBag.RestaurantId = restaurantId;
                return View(mealVM);
            }

            _context.GetRepository<Meal>().AddAsync(meal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetAllCat));
        }

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
            var restuarant = await _context.GetRepository<Restaurant>().GetOneAsync(
                criteria: c => c.Id == 5
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

    }
}