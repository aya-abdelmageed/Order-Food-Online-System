using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.Repositories;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities.Basket;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Customer.Models;
using Coupon = OrderFood.DAL.Entities.Models.Coupon;
using PaymentMethod = OrderFood.DAL.Entities.Models.PaymentMethod;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBasketRepository<CustomerBasket> _basket;
        private readonly IPaymentService _paymentService;

        public OrdersController(IUnitOfWork context, UserManager<ApplicationUser> userManager, IBasketRepository<CustomerBasket> basket, IPaymentService paymentService)
        {
            _unitOfWork = context;
            _userManager = userManager;
            this._basket = basket;
            this._paymentService = paymentService;
        }


        public async Task<IActionResult> GetCustomerOrder()
        {

            var custid = _userManager.GetUserId(User);
            //var custid = "a91aa43f-4d7d-4649-9a44-67cbd667119f";
            var custorder = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                o => o.CustomerId == custid, query => query.Include(o => o.OrderMeals)!.ThenInclude(i => i.Meal)
                );
            if (custorder == null) return NotFound();


            return View(custorder);
        }

        [HttpGet]
        public async Task<IActionResult> OrderTracking(int id)
        {

            var OrderTracking = await _unitOfWork.GetRepository<Order>().GetOneAsync(o => o.Id == id, query => query.Include(dr => dr.Driver));
            if (OrderTracking == null)
                return NotFound();

            return View(OrderTracking);
        }

        // Check Coupon
        //public async Task<IActionResult> CheckCoupon(string couponCode)
        //{
        //    var coupon = await _unitOfWork.GetRepository<Coupon>().GetOneAsync(c => c.Code == couponCode);
        //    if (coupon == null || coupon.ExpireDate < DateTime.Now)
        //    {
        //        return Json(new { isValid = false, discount = 0 });
        //    }
        //    return Json(new { isValid = true, discount = coupon.AmountPercentage });
        //}

        // checkout form
        public async Task<IActionResult> userCheckout(string? couponCode)
        {
            int CouponAmount = 0;
            int? CouponId = null;
            bool SameRestaurant = true;
            if (string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _unitOfWork.GetRepository<Coupon>().GetOneAsync(c => c.Code == couponCode);

                if (!(coupon == null || coupon.ExpireDate < DateTime.Now))
                {
                    CouponAmount = coupon.AmountPercentage;
                    CouponId = coupon.Id;
                }
            }


            var BasketId = $"Cart-{_userManager.GetUserId(User)}";
            var UserBasket = await _basket.GetBasketAsync(BasketId);
            var SubTotal = 0M;
            var MealList = new List<Meal>();
            int firstRestaurantId = 0;


            // Calculate Total Cost
            if (UserBasket!.basketItems.Count > 0)
            {
                foreach (var item in UserBasket.basketItems)
                {
                    var Meal = await _unitOfWork.GetRepository<Meal>().GetOneAsync(m => m.Id == item.Id, q=>q.Include(m=>m.Category));

                    if (Meal != null)
                    {
                        // Check if all meals belong to the same restaurant
                        if (firstRestaurantId == 0)
                        {
                            firstRestaurantId = Meal.Category!.RestaurantId; // Assuming Meal -> Category -> RestaurantId
                        }
                        else if (Meal.Category?.RestaurantId != firstRestaurantId)
                        {
                            SameRestaurant = false;
                        }

                        SubTotal += Meal.Price * item.Quantity;
                        MealList.Add(Meal);
                    }
                }
            }
            var Descount = SubTotal * CouponAmount / 100;
            var TotalCost = SubTotal - Descount + 5; // Add delivery fee
            var checkout = new CheckoutVM
            {
                Total = TotalCost,
                Descount = Descount,
                Meals = MealList,
                CouponId = CouponId
            };

            if (!SameRestaurant)
            {
                // Handle the case where meals belong to different restaurants
                ModelState.AddModelError("", "All meals in the basket must belong to the same restaurant.");
                return View(checkout); // Redirect to an error page or show a message
            }
            checkout.RestaurantId = firstRestaurantId;

            return View(checkout);
        }

        //Create Order In DB After Checkout
        public async Task<IActionResult> MakeOrder(OrderCreateVM createOrder)
        {
            var UserBasket = new CustomerBasket();
            var TotalCost = 0M;
            if (createOrder.PaymentMethod == PaymentMethod.CreditCard)
            {
                UserBasket = await _paymentService.CreateOrUpdatePaymentIntent(_userManager.GetUserId(User)!);
                if (UserBasket is null)
                    return View(createOrder);
            }
            else
            {
                var BasketId = $"Cart-{_userManager.GetUserId(User)}";
                UserBasket = await _basket.GetBasketAsync(BasketId);

                if (UserBasket is null)
                    return View(createOrder);
            }
            // Calculate Total Cost
            if (UserBasket.basketItems.Count > 0)
            {
                foreach (var item in UserBasket.basketItems)
                {
                    var Meal = await _unitOfWork.GetRepository<Meal>().GetOneAsync(m => m.Id == item.Id);
                    if (Meal is null)
                    {
                        ModelState.AddModelError("", "Error In Order Creation");
                        return View(createOrder);

                    }
                    TotalCost += Meal.Price * item.Quantity;
                }
            }

            TotalCost += 5; // Add delivery fee 

            var UpdateOrCreateBasket = await _basket.UpdateBasketAsync(UserBasket);

            if (UpdateOrCreateBasket is null)
                return View(createOrder);


            ///var FinalOrder = new OrderCreateVM()
            ///{
            ///    ShippingAddress = createOrder.ShippingAddress,
            ///    SubTotal = TotalCost,
            ///    TransactionId = UpdateOrCreateBasket.PaymentIntentId,
            ///    PaymentMethod = createOrder.PaymentMethod,
            ///    PayDate = DateTime.Now,
            ///    RestaurantId = createOrder.RestaurantId,
            ///    CustomerId = _userManager.GetUserId(User)!,
            ///};

            createOrder.CustomerId = _userManager.GetUserId(User)!;
            createOrder.PayDate = DateTime.Now;
            createOrder.SubTotal = TotalCost;
            createOrder.TransactionId = UpdateOrCreateBasket.PaymentIntentId;


            var FinalOrder = new Order()
            {
                ShippingAddress = createOrder.ShippingAddress,
                SubTotal = createOrder.SubTotal,
                TransactionId = createOrder.TransactionId,
                PaymentMethod = createOrder.PaymentMethod,
                PayDate = createOrder.PayDate,
                RestaurantId = createOrder.RestaurantId,
                CustomerId = createOrder.CustomerId,
                CouponId = createOrder.CouponId,
                OrderMeals = createOrder.Meals.Select(m => new OrderMeals()
                {
                    MealId = m.MealId,
                    Quantity = m.Quantity
                }).ToList(),
            };

            // Add Order to DB
            _unitOfWork.GetRepository<Order>().AddAsync(FinalOrder);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                ModelState.AddModelError("", "Error In Order Creation");
                return View(createOrder);
            }

            return RedirectToAction(nameof(GetCustomerOrder));
        }


    }
}