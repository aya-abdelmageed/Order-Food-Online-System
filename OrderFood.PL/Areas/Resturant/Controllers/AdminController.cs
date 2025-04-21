using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.Repositories;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Resturant.ViewModel;

namespace OrderFood.PL.Areas.Resturant.Controllers
{
    [Area("Resturant")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> UserManager { get; }

        public AdminController(IUnitOfWork context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            UserManager = userManager;
        }
        //----------------------------------------------
        public async Task<IActionResult> GetRestaurants(string nameSearch = "", string ownerSearch = "", string addressSearch = "", int PageNo = 1)
        {
            const int pageSize = 6; // Define page size for pagination

            // Fetch the restaurants based on search criteria
            var restaurants = await _context.GetRepository<Restaurant>().GetAllAsync(r =>
                (string.IsNullOrEmpty(nameSearch) || r.Name.StartsWith(nameSearch)) &&
                (string.IsNullOrEmpty(ownerSearch) || r.Owner.FirstName.StartsWith(ownerSearch)) &&
                (string.IsNullOrEmpty(addressSearch) || r.Address.Contains(addressSearch)),
                query => query.Include(p => p.Owner).OrderBy(r => r.Name)
            );

            // Pagination logic
            int totalRecords = restaurants.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Skip and take for pagination
            var paginatedRestaurants = restaurants.Skip((PageNo - 1) * pageSize).Take(pageSize).ToList();

            // Set pagination data in ViewBag
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = totalPages;
            ViewBag.NameSearch = nameSearch;
            ViewBag.OwnerSearch = ownerSearch;
            ViewBag.AddressSearch = addressSearch;

            return View(paginatedRestaurants);
        }


        //----------------------------------------------------------------------
        // GET: Admin/Restaurants/Details/id?
        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _context.GetRepository<Restaurant>().GetOneAsync(r => r.Id == id && !r.IsDelete, query => query.Include(r => r.Owner));

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        //----------------------------------------------------------------------
        // POST: Admin/Restaurants/Delete/id?
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.GetRepository<Restaurant>().GetOneAsync(r => r.Id == id && !r.IsDelete);

            if (restaurant == null)
            {
                return NotFound();
            }

            _context.GetRepository<Restaurant>().Delete(restaurant);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetRestaurants));
        }
        //-----------------------------------------------------------------------------
        // GET: Restaurant/Add
        public async Task<IActionResult> AddRestaurant()
        {
            var owners = await UserManager.GetUsersInRoleAsync("Owner");

            var model = new RestaurantViewModel();
            ViewData["Owners"] = owners;

            return View(model);
        }

        // POST: Restaurant/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRestaurant(RestaurantViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                string logoPath = null;
                if (model.Logo != null)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Admin");
                    Directory.CreateDirectory(uploadFolder); 

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Logo.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Logo.CopyToAsync(fileStream);
                    }

                    logoPath = "/images/Admin/" + fileName; 
                }

                // Create and save restaurant
                var restaurant = new Restaurant
                {
                    Name = model.Name,
                    Address = model.Address,
                    Description = model.Description,
                    HotLine = model.HotLine,
                    Logo = logoPath,
                    Lat = model.Lat,
                    Long = model.Long,
                    OwnerId = model.OwnerId
                };
                
                _context.GetRepository<Restaurant>().AddAsync(restaurant);
                await _context.SaveChangesAsync();

                return RedirectToAction("GetRestaurants");
            }
            else
            {
                // Reload owners if validation fails
                ViewData["Owners"] = await UserManager.GetUsersInRoleAsync("Owner");
                return View(model);
            }

        }
        //--------------------------------------------------------------------------
        public async Task<IActionResult> SearchRestaurants(string nameSearch = "", string ownerSearch = "", string addressSearch = "", int PageNo = 1)
        {
            var restaurant = await _context.GetRepository<Restaurant>().GetAllAsync(r =>
                        (string.IsNullOrEmpty(nameSearch) || r.Name.StartsWith(nameSearch)) &&
                        (string.IsNullOrEmpty(ownerSearch) || r.Owner.FirstName.StartsWith(ownerSearch)) &&
                        (string.IsNullOrEmpty(addressSearch) || r.Address.Contains(addressSearch)),
                        query => query.Include(p => p.Owner).OrderBy(r => r.Name)
                        );



            if (restaurant == null) return NotFound();

            /* Pagination */
            int NoOfRecordsPerPage = 6;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(restaurant.ToList().Count) / Convert.ToDouble(NoOfRecordsPerPage)));

            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;

            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;

            ViewBag.nameSearch = nameSearch;
            ViewBag.ownerSearch = ownerSearch;
            ViewBag.addressSearch = addressSearch;

            restaurant = restaurant.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return PartialView("_SearchRestaurant", restaurant);
        }
        //---------------------------------------------------------------------------------
        // Show all coupons
        public async Task<IActionResult> ViewAllCoupons()
        {
            var coupons = await _context.GetRepository<Coupon>().GetAllAsync();

            var sortedCoupons = coupons
                .OrderBy(c => c.ExpireDate < DateTime.Now)
                .ThenBy(c => c.ExpireDate)
                .ToList();

            return View(sortedCoupons);
        }
        //------------------------------------------------------
        // GET: Admin/CreateCoupon
        public async Task<IActionResult> CreateCoupon()
        {
            return View();
        }

        // POST: Admin/CreateCoupon
        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                string logoPath = null;
                if (viewModel.ImageFile != null)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Coupons");
                    Directory.CreateDirectory(uploadFolder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.ImageFile.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }

                    logoPath = "/images/Coupons/" + fileName;
                }

                var newCoupon = new Coupon
                {
                    Code = viewModel.Code,
                    AmountPercentage = viewModel.AmountPercentage,
                    ExpireDate = viewModel.ExpireDate,
                    AdminId = "3365e4ed-f8e1-49dc-8342-b3dc85426213" /*UserManager.GetUserId(User)*/,
                    Image = logoPath
                };

                _context.GetRepository<Coupon>().AddAsync(newCoupon);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewAllCoupons");
            }

            return View();
        }
        //-----------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var repo = _context.GetRepository<Coupon>();
            var coupon = await repo.GetOneAsync(i => i.Id == id);

            if (coupon != null)
            {
                repo.Delete(coupon);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewAllCoupons"); 
        }


















    }
}
