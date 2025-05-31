using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Delivery.ViewModel;
using System.Threading.Tasks;

namespace OrderFood.PL.Areas.Delivery.Controllers
{
    [Area("Delivery")]
    public class DeliveryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliveryController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> PreparedOrders()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                includes: o => o.Include(or => or.Restaurant)
                                .Include(or => or.Customer)
                                .Include(or => or.Coupon)
                                .Include(or => or.OrderMeals)!.ThenInclude(om => om.Meal)
            );

            var orderMapped = _mapper.Map<List<DeliveryOrderDetailsVM>>(orders);
            return View(orderMapped);
        }

        [HttpPost]
        public IActionResult GetOrderDetails([FromBody] DeliveryOrderDetailsVM orderDetails)
        {
            return PartialView(orderDetails);
        }

        // Get Recent Orders That are Completed
        public async Task<IActionResult> DeliveredOrders()
        {
            var userId = _userManager.GetUserId(User);
            var allOrders = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                criteria: o => o.DriverId == userId && o.OrderStatus == OrderStatus.Completed,
                includes: o => o.Include(or => or.Restaurant)
                                .Include(or => or.Customer)
                                .Include(or => or.Coupon)
                                .Include(or => or.OrderMeals)!.ThenInclude(om => om.Meal)
            );

            var orderMapped = _mapper.Map<List<DeliveryOrderDetailsVM>>(allOrders);
            return View(orderMapped);
        }

        // Accept Prepared Orders
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var order = await _unitOfWork.GetRepository<Order>().GetOneAsync(
                o => o.Id == orderId,
                q => q.Include(o => o.Coupon) // Include coupon here
            );

            if (order == null)
                return NotFound();

            order.OrderStatus = OrderStatus.Shipping;
            order.DriverId = _userManager.GetUserId(User);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(CurrentShippingOrder));
        }

        // Get All Orders That are in Shipping
        public async Task<IActionResult> CurrentShippingOrder()
        {
            var userId = _userManager.GetUserId(User);
            var allOrders = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                criteria: o => o.DriverId == userId && o.OrderStatus == OrderStatus.Shipping,
                includes: o => o.Include(or => or.Restaurant)
                                .Include(or => or.Customer)
                                .Include(or => or.Coupon)
                                .Include(or => or.OrderMeals)!.ThenInclude(om => om.Meal)
            );

            var orderMapped = _mapper.Map<List<DeliveryOrderDetailsVM>>(allOrders);
            return View(orderMapped);
        }

        // Get Order Details That are in Completed (DONE)
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var order = await _unitOfWork.GetRepository<Order>().GetOneAsync(o => o.Id == orderId);
            if (order == null)
                return NotFound();

            order.OrderStatus = OrderStatus.Completed;
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(PreparedOrders));
        }

        // Accept Order for shipping
        public async Task<IActionResult> Acceptshipping(int id)
        {
            var order = await _unitOfWork.GetRepository<Order>().GetOneAsync(
                o => o.Id == id,
                q => q.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                      .Include(o => o.Coupon) // Include coupon here
            );

            order.OrderStatus = OrderStatus.Shipping;
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        // Update order from shipping to complete
        public async Task<IActionResult> updateShippingtoComplete(int id)
        {
            var order = await _unitOfWork.GetRepository<Order>().GetOneAsync(
                o => o.Id == id,
                q => q.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                      .Include(o => o.Coupon) // Include coupon here
            );

            order.OrderStatus = OrderStatus.Completed;
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
