using OrderFood.DAL.Entities.Models;

namespace OrderFood.PL.Areas.Customer.Models
{
    public class RestaurantReviewsViewModel
    {
        public Restaurant restaurant { get; set; }
        public decimal review { get; set; }
    }
}
