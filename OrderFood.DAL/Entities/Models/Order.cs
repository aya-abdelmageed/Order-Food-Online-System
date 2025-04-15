using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class Order : BaseEntity
    {
        public string ShippingAddress { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public DateTime CreatedOrder { get; set; } = DateTime.Now;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;


        // Payment
        public string PaymentId { get; set; }
        public string? TransactionId { get; set; } // stripe transaction id
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime? PayDate { get; set; }

        // Navigation Properties For Driver
        [ForeignKey("DriverId")]
        public ApplicationUser? Driver { get; set; }
        public string? DriverId { get; set; }

        // Navigation Properties For Customer
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }
        public string CustomerId { get; set; }


        // Navigation Properties For Meals
        public ICollection<OrderMeals>? OrderMeals { get; set; } = new HashSet<OrderMeals>();

        // Navigation Properties For Coupon
        [ForeignKey("CouponId")]
        public Coupon? Coupon { get; set; }
        public int? CouponId { get; set; }
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard
    }
    public enum OrderStatus
    {
        Pending,
        Shipping,
        Completed,
        Cancelled
    }
}
