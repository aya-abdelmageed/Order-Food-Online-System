using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Delivery.ViewModel;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;

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
                criteria: O => O.OrderStatus == OrderStatus.Prepared,
                includes: O => O.Include(or => or.Restaurant).Include(or => or.Customer).Include(or => or.Coupon).Include(or => or.OrderMeals)!.ThenInclude(om => om.Meal)
                );

            var OrderMapped = _mapper.Map<List<DeliveryOrderDetailsVM>>(orders);

            return View(OrderMapped);
        }

        [HttpPost]
        public IActionResult GetOrderDetails([FromBody] DeliveryOrderDetailsVM orderDetails)
        {
            return PartialView(orderDetails);
        }

        // Get Recent Orders That are Completed
        public async Task<IActionResult> DeliveredOrders()
        {
            var UserId = _userManager.GetUserId(User);
            var allOrders = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                criteria: o=>o.DriverId == UserId && o.OrderStatus == OrderStatus.Completed,
                includes: O => O.Include(or => or.Restaurant).Include(or => or.Customer).Include(or => or.Coupon)
                                .Include(or => or.OrderMeals)!.ThenInclude(om => om.Meal)
            );

            var OrderMapped = _mapper.Map<List<DeliveryOrderDetailsVM>>(allOrders);

            return View(OrderMapped);

        }

        // Accept Prepared Orders
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var order = await _unitOfWork.GetRepository<Order>().GetOneAsync(o => o.Id == orderId);

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
            var UserId = _userManager.GetUserId(User);
            var allOrders = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                criteria: o => o.DriverId == UserId && o.OrderStatus == OrderStatus.Shipping,
                includes: O => O.Include(or => or.Restaurant).Include(or => or.Customer).Include(or => or.Coupon)
                                .Include(or => or.OrderMeals)!.ThenInclude(om => om.Meal)
            );
            var OrderMapped = _mapper.Map<List<DeliveryOrderDetailsVM>>(allOrders);
            return View(OrderMapped);
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



    }
}
