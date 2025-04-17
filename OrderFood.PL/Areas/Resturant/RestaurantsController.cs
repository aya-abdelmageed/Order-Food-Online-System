using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Models;

namespace OrderFood.PL.Areas.Resturant
{
    [Area("Resturant")]
    public class RestaurantsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var restuarant = await unitOfWork.GetRepository<Restaurant>().GetOneAsync(
                criteria: c => c.Id == 2
                );
            var model = new UploadViewModel
            {
                Restaurant = restuarant
            };
            return (View(model));
        }

        [HttpPost]
        //update restaurant info
        public async Task<IActionResult> Settings(UploadViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }
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

                model.Restaurant.Logo = "/images/restaurant/" + uniqueFileName;
            }
            if (ModelState.IsValid)
            {
                unitOfWork.GetRepository<Restaurant>().Update(model.Restaurant);
                await unitOfWork.SaveChangesAsync();
                return RedirectToAction("Settings");
            }
            else
            {
                return View(model);

            }

        }


    }
}
        //    // GET: Resturant/Restaurants
        //    public async Task<IActionResult> Index(int id)
        //    {
        //        var restaurant = await unitOfWork.GetRepository<Restaurant>().GetOneAsync(
        //            criteria: c => c.Id == id,
        //            includes: c => c.Include(c => c.Owner).Include(c => c.Categories).ThenInclude(cat => cat.Meals)
        //            );

        //        return View(restaurant);
        //    }

        //    // GET: Resturant/Restaurants/Details/5
        //    public async Task<IActionResult> Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var restaurant = await _context.Restaurants
        //            .Include(r => r.Owner)
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (restaurant == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(restaurant);
        //    }

        //    // GET: Resturant/Restaurants/Create
        //    public IActionResult Create()
        //    {
        //        ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
        //        return View();
        //    }

        //    // POST: Resturant/Restaurants/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("Name,Address,HotLine,Logo,Long,Lat,OwnerId,Id,IsDelete")] Restaurant restaurant)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(restaurant);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", restaurant.OwnerId);
        //        return View(restaurant);
        //    }

        //    // GET: Resturant/Restaurants/Edit/5
        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var restaurant = await _context.Restaurants.FindAsync(id);
        //        if (restaurant == null)
        //        {
        //            return NotFound();
        //        }
        //        ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", restaurant.OwnerId);
        //        return View(restaurant);
        //    }

        //    // POST: Resturant/Restaurants/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, [Bind("Name,Address,HotLine,Logo,Long,Lat,OwnerId,Id,IsDelete")] Restaurant restaurant)
        //    {
        //        if (id != restaurant.Id)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(restaurant);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!RestaurantExists(restaurant.Id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", restaurant.OwnerId);
        //        return View(restaurant);
        //    }

        //    // GET: Resturant/Restaurants/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var restaurant = await _context.Restaurants
        //            .Include(r => r.Owner)
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (restaurant == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(restaurant);
        //    }

        //    // POST: Resturant/Restaurants/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var restaurant = await _context.Restaurants.FindAsync(id);
        //        if (restaurant != null)
        //        {
        //            _context.Restaurants.Remove(restaurant);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool RestaurantExists(int id)
        //    {
        //        return _context.Restaurants.Any(e => e.Id == id);
        //    }
        //}

    
