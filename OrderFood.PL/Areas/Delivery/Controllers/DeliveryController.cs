using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.PL.Areas.Delivery.ViewModel;
using System.Threading.Tasks;

namespace OrderFood.PL.Areas.Delivery.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> PreparedOrders()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                criteria: O => O.OrderStatus == OrderStatus.Prepared,
                includes: O => O.Include(or => or.Restaurant).Include(or => or.Customer).Include(or => or.OrderMeals)!.ThenInclude(om => om.Meal).Include(or => or.Coupon)
                );

            var OrderMapped = _mapper.Map<List<DeliveryOrderDetailsVM>>(orders);

            return View();
        }
    }
}
