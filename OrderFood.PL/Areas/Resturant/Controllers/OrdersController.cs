using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;

namespace OrderFood.PL.Areas.Resturant.Controllers
{
    [Area("Resturant")]
    [Authorize(Roles = "Owner")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _context;

        public OrdersController(IUnitOfWork context)
        {
            _context = context;
        }

        // GET: Resturant/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.GetRepository<Order>().GetOneAsync(
                O => O.Id == id,
                q => q.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                      .Include(o => o.Coupon)
            );

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Resturant/Orders/RejectOrder/5
        [HttpPost]
        public async Task<IActionResult> RejectOrder(int id)
        {
            var order = await _context.GetRepository<Order>().GetOneAsync(
                O => O.Id == id,
                q => q.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                      .Include(o => o.Coupon)
            );

            if (order == null)
                return NotFound("Order not found.");

            order.OrderStatus = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return Ok("Order rejected successfully.");
        }

        // POST: Resturant/Orders/AcceptOrder/5
        [HttpPost]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var order = await _context.GetRepository<Order>().GetOneAsync(
                O => O.Id == id,
                q => q.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                      .Include(o => o.Coupon)
            );

            if (order == null)
                return NotFound("Order not found.");

            order.OrderStatus = OrderStatus.Preparing;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: Resturant/Orders/DoneOrder/5
        [HttpPost]
        public async Task<IActionResult> DoneOrder(int id)
        {
            var order = await _context.GetRepository<Order>().GetOneAsync(
                O => O.Id == id,
                q => q.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                      .Include(o => o.Coupon)
            );

            if (order == null)
                return NotFound("Order not found.");

            order.OrderStatus = OrderStatus.Prepared;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
