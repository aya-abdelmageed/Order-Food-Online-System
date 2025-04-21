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
       
    }
}
