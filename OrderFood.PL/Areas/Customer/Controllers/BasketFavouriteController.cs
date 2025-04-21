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
            string userId = "Cart-306e5a79-171d-49c5-96c3-3e63953555a7";

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


        public async Task<IActionResult> UpdateCustomerCart(BasketItem customerBasket)
        {
            string userId = "Cart-306e5a79-171d-49c5-96c3-3e63953555a7";
            var UserBasket = await _basket.GetBasketAsync(userId);
            if (UserBasket is null)
                UserBasket = new CustomerBasket(userId);


            UserBasket.basketItems.Add(customerBasket);

            var UpdateOrCreateBasket = await _basket.UpdateBasketAsync(UserBasket);
            if (UpdateOrCreateBasket is null)
            {
                TempData["Error"] = "Failed to update the basket.";
            }
            else
            {
                TempData["Success"] = "Basket updated successfully.";
            }

            return RedirectToAction(nameof(GetCustomerCart));
        }


        public async Task<IActionResult> DeleteCart()
        {
            string userId = "Cart-306e5a79-171d-49c5-96c3-3e63953555a7";
            var result = await _basket.DeleteBasketAsync(userId);
            if (result)
            {
                TempData["Success"] = "Basket deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete the basket.";
            }
            return RedirectToAction(nameof(GetCustomerCart));
        }


        // =============================================== Favourite ===============================================================
        
        public async Task<IActionResult> GetCustomerFavourite()
        {
            //var userId = _userManager.GetUserId(User);
            string userId = "Favourite-306e5a79-171d-49c5-96c3-3e63953555a7";
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

        public async Task<IActionResult> UpdateCustomerFavourite(FavouriteItem customerFavourite)
        {
            string userId = "Favourite-306e5a79-171d-49c5-96c3-3e63953555a7";
            var UserFavourite = await _fvourite.GetBasketAsync(userId);

            if (UserFavourite is null)
                UserFavourite = new CustomerFavourite(userId);

            UserFavourite.FavouriteItems.Add(customerFavourite);

            var UpdateOrCreateBasket = await _fvourite.UpdateBasketAsync(UserFavourite);

            if (UpdateOrCreateBasket is null)
            {
                TempData["Error"] = "Failed to update the favourite.";
            }
            else
            {
                TempData["Success"] = "Favourite updated successfully.";
            }
            return RedirectToAction(nameof(GetCustomerFavourite));
        }

        public async Task<IActionResult> DeleteFavourite()
        {
            string userId = "Favourite-306e5a79-171d-49c5-96c3-3e63953555a7";
            var result = await _fvourite.DeleteBasketAsync(userId);
            if (result)
            {
                TempData["Success"] = "Favourite deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete the favourite.";
            }
            return RedirectToAction(nameof(GetCustomerFavourite));
        }


    }
}
