using OrderFood.DAL.Entities.Models;

namespace OrderFood.PL.Areas.Customer.Models
{
    public class OrderCreateVM
    {
        public string ShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public string? TransactionId { get; set; } // stripe transaction id
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? PayDate { get; set; }
        public int RestaurantId { get; set; }
        public string? CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CouponId { get; set; }
        public List<MealToOrder> Meals { get; set; } = new List<MealToOrder>();

    }

    public class MealToOrder
    {
        public int MealId { get; set; }
        public int Quantity { get; set; }
    }
}
