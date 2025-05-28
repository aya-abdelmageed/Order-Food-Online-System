using OrderFood.DAL.Entities.Models;

namespace OrderFood.PL.Areas.Customer.Models
{
    public class CheckoutVM
    {
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public int? CouponId { get; set; }
        public int RestaurantId { get; set; }
        public List<CartItemVM> CartItems { get; set; } = new List<CartItemVM>();

        // User information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public class CartItemVM
    {
        public Meal Meal { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Meal.Price * Quantity;
    }

}
