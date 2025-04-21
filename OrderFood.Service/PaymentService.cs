using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Basket;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository<CustomerBasket> _basket;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration, IBasketRepository<CustomerBasket> basket, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basket = basket;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string UserId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            var BasketId = $"Cart-{UserId}";

            var UserBasket = await _basket.GetBasketAsync(BasketId);

            if (UserBasket is null)
                return null;

            var TotalCost = 0M;
            // Calculate Total Cost
            if (UserBasket.basketItems.Count > 0)
            {
                foreach (var item in UserBasket.basketItems)
                {
                    var Meal = await _unitOfWork.GetRepository<Meal>().GetOneAsync(m=>m.Id == item.Id);
                    if (Meal is null)
                        return null;
                    TotalCost += Meal.Price * item.Quantity;
                }
            }

            TotalCost += 5; // Add delivery fee 

            // Create Payment Intent
            var Service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(UserBasket.PaymentIntentId)) // Create New Payment Itent
            {
                var Option = new PaymentIntentCreateOptions()
                {
                    Amount = (long) TotalCost * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await Service.CreateAsync(Option);
                UserBasket.ClientSecret = paymentIntent.ClientSecret;
                UserBasket.PaymentIntentId = paymentIntent.Id;
            }
            else // Update Current Payment Intent
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)TotalCost * 100,
                };
                paymentIntent = await Service.UpdateAsync(UserBasket.PaymentIntentId, Options);
                UserBasket.ClientSecret = paymentIntent.ClientSecret;
                UserBasket.PaymentIntentId = paymentIntent.Id;
            }

            var UpdateOrCreateBasket = await _basket.UpdateBasketAsync(UserBasket);
            
            if (UpdateOrCreateBasket is null)
                return null;
            else
                return UserBasket;
        }
    }
}
