using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Customer.Models;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ReviewsController : Controller
    {
        public IUnitOfWork unitOfWork { get; private set; }
        public UserManager<ApplicationUser> userManager { get; private set; }

        public ReviewsController(IUnitOfWork _unitOfWork, UserManager<ApplicationUser> _userManager)
        {
            unitOfWork = _unitOfWork;
            userManager = _userManager;
        }

        // GET: ReviewsController/GetCustomerReviews 
        public async Task<IActionResult> GetCustomerReviews()
        {
            var user = await userManager.GetUserAsync(User);
            var reviews = await unitOfWork.GetRepository<Review>().GetAllAsync(i => i.CustomerId == user.Id, i => i.Include(r => r.Restaurant)
            );
            var FilteredReviews = reviews.Where(r => !r.IsDelete).ToList();
            return View(reviews);
                
        }



        // GET: ReviewsController/Details/5
        public async Task <IActionResult> ReviewDetails(int id)
        {
            var review = await unitOfWork.GetRepository<Review>().GetOneAsync(i => i.Id == id,
            query => query
           .Include(r => r.Restaurant));
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // GET: ReviewsController/Create
        public async Task <IActionResult> Create(int restaurantId)
        {
            var user = await userManager.GetUserAsync(User);



            //get orders that this customer made from this restaurant
            var orders = await unitOfWork.GetRepository<Order>().GetAllAsync();

            var hasOredered = orders.Any(i => i.RestaurantId == restaurantId && i.CustomerId == user.Id);

            //Console.WriteLine($"User {user.Id} has {orders.Count()} orders from restaurant {restaurantId}");

            if (!hasOredered)
            {
                TempData["AlertMessage"] = "You must order from this restaurant before leaving a review.";
                return RedirectToAction("GetMenu", "Customer", new { id = restaurantId });
            }


            return View(new ReviewViewModel { RestaurantId = restaurantId });
        }

        // POST: ReviewsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(ReviewViewModel model)
        {
           if(!ModelState.IsValid)
            {
                
                return View(model);
            
            }

            var user = await userManager.GetUserAsync(User);
            Review newReview = new Review
            {
                RestaurantId = model.RestaurantId,
                CustomerId = user.Id,
                Rate = model.Rate,
                Comment = model.Comment
            };
            unitOfWork.GetRepository<Review>().AddAsync(newReview);
            await unitOfWork.SaveChangesAsync();
            return RedirectToAction("GetCustomerReviews", "Reviews", new { area = "Customer" });
        }

        // GET: ReviewsController/Edit/5
        public async Task <IActionResult> EditReview(int id)
        {
            var review = await unitOfWork.GetRepository<Review>().GetOneAsync(i => i.Id == id, query => query.Include(r => r.Restaurant));
            
            return View(review);
        }

        // POST: ReviewsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task  <IActionResult> EditReview(int id, Review review)
        {
            var existingReview = await unitOfWork.GetRepository<Review>().GetOneAsync(i => i.Id == id);
            if (existingReview == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(review);
            }
            existingReview.Rate = review.Rate;
            existingReview.Comment = review.Comment;
            unitOfWork.GetRepository<Review>().Update(existingReview);
            await unitOfWork.SaveChangesAsync();
            return RedirectToAction("GetCustomerReviews", "Reviews", new { area = "Customer" });
        }

        // GET: ReviewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var reviewToDelete = await unitOfWork.GetRepository<Review>().GetOneAsync(i => i.Id == id);
            if (reviewToDelete == null)
            {
                return NotFound();
            }

            reviewToDelete.IsDelete = true;
            unitOfWork.GetRepository<Review>().Update(reviewToDelete);
            await unitOfWork.SaveChangesAsync();

            return Ok(); 
        }


    }
}
