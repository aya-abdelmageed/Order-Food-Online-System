using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Basket;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public async Task<IActionResult> CreateOrUpdatePaymentIntent(string UserId)
        {
            var customerBasket = await _paymentService.CreateOrUpdatePaymentIntent(UserId);

            if (customerBasket == null)
                return BadRequest();

            //return Ok(MappedBasket);

            return await _paymentService.CreateOrUpdatePaymentIntent(UserId) is null ? BadRequest(new {Message="Error In Payment"}) : Ok(new { Message = "Purchase Successfully" });
        }


    }
}
