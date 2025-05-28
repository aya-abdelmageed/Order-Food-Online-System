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
            var custorder = await _unitOfWork.GetRepository<Order>().GetAllAsync(
                o => o.CustomerId == custid,
                query => query
                            .Include(o => o.OrderMeals)!
                                .ThenInclude(om => om.Meal)
                            .Include(o => o.Restaurant)
                            .Include(o => o.Coupon) 
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
        public async Task<IActionResult> CheckCoupon(string couponCode)
        {
            var coupon = await _unitOfWork.GetRepository<Coupon>().GetOneAsync(c => c.Code == couponCode);
            if (coupon == null || coupon.ExpireDate < DateTime.Now)
            {
                return Json(new { isValid = false, discount = 0 });
            }
            return Json(new { isValid = true, discount = coupon.AmountPercentage, id = coupon.Id });
        }

        // checkout form
        public async Task<IActionResult> userCheckout(string? couponCode)
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);

            // Coupon validation
            decimal CouponAmount = 0m;
            int? CouponId = null;
            bool SameRestaurant = true;

            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _unitOfWork.GetRepository<Coupon>().GetOneAsync(c => c.Code == couponCode);
                if (!(coupon == null || coupon.ExpireDate < DateTime.Now))
                {
                    CouponAmount = coupon.AmountPercentage;
                    CouponId = coupon.Id;
                }
            }

            // Basket processing
            var BasketId = $"Cart-{_userManager.GetUserId(User)}";
            var UserBasket = await _basket.GetBasketAsync(BasketId);
            var SubTotal = 0M;
            var CartItems = new List<CartItemVM>();
            int firstRestaurantId = 0;

            // Calculate Total Cost
            if (UserBasket!.basketItems.Count > 0)
            {
                foreach (var basketItem in UserBasket.basketItems)
                {
                    var Meal = await _unitOfWork.GetRepository<Meal>().GetOneAsync(m => m.Id == basketItem.Id, q => q.Include(m => m.Category));

                    if (Meal != null)
                    {
                        // Check if all meals belong to the same restaurant
                        if (firstRestaurantId == 0)
                        {
                            firstRestaurantId = Meal.Category!.RestaurantId;
                        }
                        else if (Meal.Category?.RestaurantId != firstRestaurantId)
                        {
                            SameRestaurant = false;
                        }

                        SubTotal += Meal.Price * basketItem.Quantity;
                        CartItems.Add(new CartItemVM
                        {
                            Meal = Meal,
                            Quantity = basketItem.Quantity
                        });
                    }
                }
            }

            if (!SameRestaurant)
            {
                ModelState.AddModelError("", "All meals in the basket must belong to the same restaurant.");
                var errorCheckout = new CheckoutVM
                {
                    Total = 0,
                    Discount = 0,
                    CartItems = new List<CartItemVM>(),
                    CouponId = null,
                    RestaurantId = 0,
                    FirstName = user?.FirstName,
                    LastName = user?.LastName,
                    UserName = user?.UserName,
                    Email = user?.Email,
                    Address = user?.Address
                };
                return View(errorCheckout);
            }

            var Discount = SubTotal * (CouponAmount / 100m);
            var TotalCost = SubTotal - Discount + 5; // Add delivery fee

            var checkout = new CheckoutVM
            {
                Total = TotalCost,
                Discount = Discount,
                CartItems = CartItems,  
                CouponId = CouponId,
                RestaurantId = firstRestaurantId,
                FirstName = user?.FirstName,
                LastName = user?.LastName,
                UserName = user?.UserName,
                Email = user?.Email,
                Address = user?.Address
            };

            return View(checkout);
        }


        //Create Order In DB After Checkout
        [HttpPost]
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
            var mealsBasket = new List<OrderMeals>();
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
                    mealsBasket.Add(new OrderMeals()
                    {
                        MealId = Meal.Id,
                        Quantity = item.Quantity
                    });
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
            createOrder.Total = TotalCost;
            createOrder.TransactionId = UpdateOrCreateBasket.PaymentIntentId;


            var coupon = await _unitOfWork.GetRepository<Coupon>().GetOneAsync(c => c.Code == createOrder.CouponId);


            var FinalOrder = new Order();
            if (coupon.Id == 0)
            {
                FinalOrder = new Order()
                {
                    ShippingAddress = createOrder.ShippingAddress,
                    SubTotal = createOrder.Total ?? 0,
                    TransactionId = createOrder.TransactionId,
                    PaymentMethod = createOrder.PaymentMethod,
                    PayDate = createOrder.PayDate,
                    RestaurantId = createOrder.RestaurantId,
                    CustomerId = createOrder.CustomerId,
                    OrderMeals = mealsBasket
                };
            }
            else
            {
                FinalOrder = new Order()
                {
                    ShippingAddress = createOrder.ShippingAddress,
                    SubTotal = createOrder.Total ?? 0,
                    TransactionId = createOrder.TransactionId,
                    PaymentMethod = createOrder.PaymentMethod,
                    PayDate = createOrder.PayDate,
                    RestaurantId = createOrder.RestaurantId,
                    CustomerId = createOrder.CustomerId,
                    CouponId = coupon?.Id,
                    OrderMeals = mealsBasket
                };
            }            

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



        // ================================================================//
        [HttpGet]
        public async Task<IActionResult> GetCheckoutSummary(string? couponCode)
        {
            decimal couponAmount = 0m;
            decimal discount = 0;
            int? couponId = null;

            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _unitOfWork.GetRepository<Coupon>().GetOneAsync(c => c.Code == couponCode);
                if (coupon != null && coupon.ExpireDate >= DateTime.Now)
                {
                    couponAmount = coupon.AmountPercentage;
                    couponId = coupon.Id;
                }
            }

            var basketId = $"Cart-{_userManager.GetUserId(User)}";
            var userBasket = await _basket.GetBasketAsync(basketId);

            decimal subTotal = 0;

            if (userBasket != null && userBasket.basketItems.Any())
            {
                foreach (var item in userBasket.basketItems)
                {
                    var meal = await _unitOfWork.GetRepository<Meal>()
                        .GetOneAsync(m => m.Id == item.Id, q => q.Include(m => m.Category));
                    if (meal != null)
                    {
                        subTotal += meal.Price * item.Quantity;
                    }
                }
            }

            discount = subTotal * (couponAmount / 100m);
            decimal total = subTotal - discount + 5; // +5 for delivery

            return Json(new
            {
                isValid = true,
                total = total,
                discount = discount,
                couponId = couponId
            });
        }

    }
}