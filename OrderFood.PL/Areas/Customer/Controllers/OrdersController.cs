using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public OrdersController(IUnitOfWork context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;


        }


        public async Task<IActionResult> GetCustomerOrder()
        {

            var custid = _userManager.GetUserId(User);
            
            var custorder = await _context.GetRepository<Order>().GetAllAsync(
                o => o.CustomerId == custid, query => query.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                );
            if (custorder == null) return NotFound();

            return View(custorder);
        }

        [HttpGet]
        public async Task<IActionResult> OrderTracking(int id)
        {

            var OrderTracking = await _context.GetRepository<Order>().GetOneAsync(o => o.Id == id, query => query.Include(dr => dr.Driver));
            if (OrderTracking == null)
                return NotFound();

            return View(OrderTracking);
        }
    }
}