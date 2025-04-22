using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Basket;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Customer.Models;
using Stripe;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository<CustomerBasket> _basket;

        public PaymentController(IPaymentService paymentService, IMapper mapper, IUnitOfWork unitOfWork, IBasketRepository<CustomerBasket> basket)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _basket = basket;
        }

        public async Task<IActionResult> CreateOrUpdatePaymentIntent(string UserId)
        {
            //var customerBasket = await _paymentService.CreateOrUpdatePaymentIntent(UserId);


            //if (customerBasket == null)
            //    return BadRequest();

            //var isOrderCreated = await CreateOrder(customerBasket);

            //if (!isOrderCreated)
            //    return BadRequest(new { Message = "Error In Order Creation" });

            //return Ok(customerBasket);
            return await _paymentService.CreateOrUpdatePaymentIntent(UserId) is null ? BadRequest(new {Message="Error In Payment"}) : Ok(new { Message = "Purchase Successfully" });
        }

        // Add order into DB
        //public async Task<IActionResult> CreateOrder(OrderCreateVM order)
        //{
        //    CustomerBasket basket;
        //    var coupon = await _unitOfWork.GetRepository<DAL.Entities.Models.Coupon>().GetOneAsync(c=>c.Code == order.Coupon);
        //    if(coupon == null || coupon.ExpireDate < DateTime.Now)
        //    {
        //        return BadRequest(new { Message = "Coupon is not valid" });
        //    }
        //    if(order.PaymentMethod == DAL.Entities.Models.PaymentMethod.CreditCard)
        //    {
        //        basket = await _paymentService.CreateOrUpdatePaymentIntent(order.CustomerId);
        //        if (basket == null)
        //            return BadRequest(new { Message = "Error In Payment" });
        //    }
        //    else
        //    {
        //        basket = await _basket.GetBasketAsync($"Cart-{order.CustomerId}");
        //        if (basket == null)
        //            return BadRequest(new { Message = "Error In Payment" });
        //    }





        //}
    
    
    
    }
}
