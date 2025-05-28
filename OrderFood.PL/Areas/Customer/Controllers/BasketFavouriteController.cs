using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Basket;
using OrderFood.DAL.Entities.User;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BasketFavouriteController : Controller
    {
        private readonly IBasketRepository<CustomerBasket> _basket;
        private readonly IBasketRepository<CustomerFavourite> _fvourite;
        private readonly UserManager<ApplicationUser> _userManager;

        public BasketFavouriteController(IBasketRepository<CustomerBasket> basket, IBasketRepository<CustomerFavourite> favourite, UserManager<ApplicationUser> userManager)
        {
            _basket = basket;
            _fvourite = favourite;
            _userManager = userManager;
        }

        // =============================================== Cart ===============================================================
        public async Task<IActionResult> GetCustomerCart()
        {
            //var userId = _userManager.GetUserId(User);
            string userId = $"Cart-{_userManager.GetUserId(User)}";

            var UserBasket = await _basket.GetBasketAsync(userId);

            if (UserBasket is null)
            {
                return View(new CustomerBasket(userId));
            }
            else
            {
                return View(UserBasket);
            }
        }


        public async Task<IActionResult> UpdateCustomerCart([FromBody]BasketItem customerBasket)
        {
            string userId = $"Cart-{_userManager.GetUserId(User)}";
            var UserBasket = await _basket.GetBasketAsync(userId);
            if (UserBasket is null)
                UserBasket = new CustomerBasket(userId);

            // check if item is exists in the basket
            var existingItem = UserBasket.basketItems.FirstOrDefault(x => x.Id == customerBasket.Id);
            if (existingItem != null)
            {
                // Update the existing item
                existingItem.Quantity = customerBasket.Quantity;
            }
            else
            {
                UserBasket.basketItems.Add(customerBasket);
            }


            var UpdateOrCreateBasket = await _basket.UpdateBasketAsync(UserBasket);

            if (UpdateOrCreateBasket is null)
            {
                TempData["Error"] = "Failed to update the basket.";
            }
            else
            {
                TempData["Success"] = "Basket updated successfully.";
            }

            return Json(new { success = true });
        }


        public async Task<IActionResult> DeleteCart(int Id)
        {
            string userId = $"Cart-{_userManager.GetUserId(User)}";

            var basket = await _basket.GetBasketAsync(userId);
            var existingItem = basket.basketItems.FirstOrDefault(x => x.Id == Id);
            if (existingItem != null)
            {
                basket.basketItems.Remove(existingItem);
            }
            var result = await _basket.UpdateBasketAsync(basket);

            return Json(new { success = true });
        }


        // =============================================== Favourite ===============================================================

        public async Task<IActionResult> GetCustomerFavourite()
        {
            //var userId = _userManager.GetUserId(User);
            string userId = $"Favourite-{_userManager.GetUserId(User)}";
            var UserFavourite = await _fvourite.GetBasketAsync(userId);
            if (UserFavourite is null)
            {
                return View(new CustomerFavourite(userId));
            }
            else
            {
                return View(UserFavourite);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateCustomerFavourite([FromBody]FavouriteItem customerFavourite)
        //{
        //    string userId = $"Favourite-{_userManager.GetUserId(User)}";
        //    var UserFavourite = await _fvourite.GetBasketAsync(userId);

        //    if (UserFavourite is null)
        //        UserFavourite = new CustomerFavourite(userId);

        //    // check if item is exists in the favourite
        //    var existingItem = UserFavourite.FavouriteItems.FirstOrDefault(x => x.Id == customerFavourite.Id);

        //    if (existingItem is null)
        //        UserFavourite.FavouriteItems.Add(customerFavourite);


        //    var UpdateOrCreateBasket = await _fvourite.UpdateBasketAsync(UserFavourite);

        //    if (UpdateOrCreateBasket is null)
        //    {
        //        TempData["Error"] = "Failed to update the favourite.";
        //    }
        //    else
        //    {
        //        TempData["Success"] = "Favourite updated successfully.";
        //    }
        //    return Json(new { success = true });
        //}

        [HttpPost]
        public async Task<IActionResult> UpdateCustomerFavourite([FromBody] FavouriteItem customerFavourite)
        {
            string userId = $"Favourite-{_userManager.GetUserId(User)}";
            var UserFavourite = await _fvourite.GetBasketAsync(userId);

            if (UserFavourite == null)
                UserFavourite = new CustomerFavourite(userId);

            // Check if item exists in the favourite
            var existingItem = UserFavourite.FavouriteItems.FirstOrDefault(x => x.Id == customerFavourite.Id);

            if (existingItem != null)
            {
                // Item exists, remove it (toggle off)
                UserFavourite.FavouriteItems.Remove(existingItem);
            }
            else
            {
                // Item doesn't exist, add it (toggle on)
                UserFavourite.FavouriteItems.Add(customerFavourite);
            }

            var updateOrCreateBasket = await _fvourite.UpdateBasketAsync(UserFavourite);

            if (updateOrCreateBasket == null)
            {
                return Json(new { success = false, message = "Failed to update the favourite." });
            }

            bool isAdded = existingItem == null; // true if item was added, false if removed
            return Json(new { success = true, isAdded = isAdded });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFavourite(int Id)
        {
            string userId = $"Favourite-{_userManager.GetUserId(User)}";
            var favourite = await _fvourite.GetBasketAsync(userId); 
            var existingItem = favourite.FavouriteItems.FirstOrDefault(x => x.Id == Id);

            if (existingItem != null)
            {
                favourite.FavouriteItems.Remove(existingItem);
            }
            var result = await _fvourite.UpdateBasketAsync(favourite);

            return Json(new { success = true });
        }
        //---------------------------------------------------------
        

    }
}
