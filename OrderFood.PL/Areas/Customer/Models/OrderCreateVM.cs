using OrderFood.DAL.Entities.Models;

namespace OrderFood.PL.Areas.Customer.Models
{
    public class OrderCreateVM
    {
        public string ShippingAddress { get; set; }
        public decimal? Total { get; set; }
        public string? TransactionId { get; set; } // stripe transaction id
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? PayDate { get; set; }
        public int RestaurantId { get; set; }
        public string? CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CouponId { get; set; }
    }

    public class MealToOrder
    {
        public int MealId { get; set; }
        public int Quantity { get; set; }
    }
}
