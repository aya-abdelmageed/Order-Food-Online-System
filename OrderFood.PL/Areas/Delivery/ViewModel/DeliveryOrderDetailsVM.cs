using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFood.PL.Areas.Delivery.ViewModel
{
    public class DeliveryOrderDetailsVM
    {
        public int Id { get; set; }
        public string ShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CreatedOrder { get; set; }
        public int? AmountPercentageCoupon { get; set; }

        public decimal getTotal()
        {
            return SubTotal - (SubTotal * (AmountPercentageCoupon ?? 0) / 100) + 5;
            // 5 is the delivery fee
        }

        public OrderStatus OrderStatus { get; set; }

        public decimal Lat { get; set; }
        public decimal Long { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public RestaurantVM Restaurant { get; set; } 

        public UserVM Customer { get; set; }

        public ICollection<OrderMealsVM> OrderMeals { get; set; }

    }
}
