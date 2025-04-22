using OrderFood.DAL.Entities.Models;

namespace OrderFood.PL.Areas.Customer.Models
{
    public class CheckoutVM
    {
        public decimal Total { get; set; }
        public decimal Descount { get; set; }
        public int? CouponId { get; set; }
        public int RestaurantId { get; set; }
        public List<Meal> Meals { get; set; } 

    }
}
