using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Admin.Models;
using OrderFood.PL.Areas.Resturant.ViewModel;

namespace OrderFood.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles= "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        // GET: AdminController Get All Users Data
        public async Task<IActionResult> GetAll()
        {
            //create list of roles i want to get tyhe user with it
            List<string>roles = new List<string>([
                "Admin",
                "Owner",
                "Delivery"
            ]);
            //create list of users to keep track of users in that roles
            var UsersInRoles = new List<UserRoleViewModel>();
            var AuthorizedAdmin = await _userManager.GetUserAsync(User);

            //loop to make sure role exists and then get the users with that role
            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    var x = await _userManager.GetUsersInRoleAsync(role);
                    //add the users to the list with thier roles
                    foreach(var i in x)
                    {
                        if (i != AuthorizedAdmin && !i.IsDeleted )
                        {
                            UsersInRoles.Add(new UserRoleViewModel
                            {
                                user = i,
                                role = role
                            });
                        }
                    }

                }
            }
            return View(UsersInRoles);
        }

        //// GET: AdminController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: AdminController/Create
        // //create Delivery of Admin
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = new List<string>{
                "Admin",
                "Delivery",
            };

            ViewBag.Roles = new SelectList(roles);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewUserViewModel e)
        {
            if (ModelState.IsValid)
            {
                var username = e.Email.Split('@')[0];
                var user = new ApplicationUser
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Address = e.Address,
                    DateOfBirth = e.DateOfBirth,
                    UserName = username,
                };

                var result = await _userManager.CreateAsync(user, e.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, e.Role);
                    TempData["Success"] = "User created successfully!";
                    return RedirectToAction(nameof(GetAll));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            var role = new List<string> {
                "Admin",
                "Delivery",
                };

            ViewBag.Roles = new SelectList(role);
            return View(e);
        }


        
        // GET: AdminController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: AdminController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound();

                user.IsDeleted = true;

                await _userManager.UpdateAsync(user);
                TempData["Success"] = "User deleted successfully!";
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(GetAll));
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Error deleting user: " + ex.Message;
                return RedirectToAction(nameof(GetAll));
            }
        }




        //----------------------------------------------
        public async Task<IActionResult> GetRestaurants(string nameSearch = "", string ownerSearch = "", string addressSearch = "", int PageNo = 1)
        {
            const int pageSize = 6; // Define page size for pagination

            // Fetch the restaurants based on search criteria
            var restaurants = await _unitOfWork.GetRepository<Restaurant>().GetAllAsync(r =>
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
            var restaurant = await _unitOfWork.GetRepository<Restaurant>().GetOneAsync(r => r.Id == id && !r.IsDelete, query => query.Include(r => r.Owner));

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
            var restaurant = await _unitOfWork.GetRepository<Restaurant>().GetOneAsync(r => r.Id == id && !r.IsDelete);

            if (restaurant == null)
            {
                return NotFound();
            }

            _unitOfWork.GetRepository<Restaurant>().Delete(restaurant);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(GetRestaurants));
        }
        //-----------------------------------------------------------------------------



        // GET: Restaurant/Add
        public async Task<IActionResult> AddRestaurant()
        {
            var owners = await _userManager.GetUsersInRoleAsync("Owner");

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

                _unitOfWork.GetRepository<Restaurant>().AddAsync(restaurant);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("GetRestaurants");
            }
            else
            {
                // Reload owners if validation fails
                ViewData["Owners"] = await _userManager.GetUsersInRoleAsync("Owner");
                return View(model);
            }

        }
        //--------------------------------------------------------------------------






        public async Task<IActionResult> SearchRestaurants(string nameSearch = "", string ownerSearch = "", string addressSearch = "", int PageNo = 1)
        {
            var restaurant = await _unitOfWork.GetRepository<Restaurant>().GetAllAsync(r =>
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
            var coupons = await _unitOfWork.GetRepository<Coupon>().GetAllAsync();

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

                _unitOfWork.GetRepository<Coupon>().AddAsync(newCoupon);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("ViewAllCoupons");
            }

            return View();
        }
        //-----------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var repo = _unitOfWork.GetRepository<Coupon>();
            var coupon = await repo.GetOneAsync(i => i.Id == id);

            if (coupon != null)
            {
                repo.Delete(coupon);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction("ViewAllCoupons");
        }
        //--------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> RegisterOwner(RegisterOwnerViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("AddRestaurant", "Admin");

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleExists = await _roleManager.RoleExistsAsync("Owner");
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Owner"));
                }

                await _userManager.AddToRoleAsync(user, "Owner");

                TempData["Success"] = "Owner registered and role assigned successfully!";
                TempData["OwnerCreated"] = true;
                return RedirectToAction("AddRestaurant", "Admin");
            }

            foreach (var error in result.Errors)
            {
                TempData["Error"] += error.Description + " ";
            }

            return RedirectToAction("AddRestaurant", "Admin");
        }





    }
}
