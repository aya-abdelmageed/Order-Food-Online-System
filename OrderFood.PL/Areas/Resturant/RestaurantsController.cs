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
        //Get the Restaurant info to Edit
        public async Task<IActionResult> Settings()
        {
            var restuarant = await unitOfWork.GetRepository<Restaurant>().GetOneAsync(
                criteria: c => c.Id == 5
                );
            var model = new UploadViewModel
            {
                Restaurant = restuarant
            };
            return (View(model));
        }

        [HttpPost]
        //update restaurant info including uploade files for logo
        public async Task<IActionResult> Settings(UploadViewModel model)
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
     
