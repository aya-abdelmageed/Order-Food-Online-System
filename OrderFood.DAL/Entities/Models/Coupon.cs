using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class Coupon : BaseEntity
    {
        public int AmountPercentage { get; set; }
        public string Code { get; set; }
        public string? Image {  get; set; }
        public DateTime ExpireDate { get; set; }

        // Navigation property For Admin
        [ForeignKey("AdminId")]
        public ApplicationUser? Admin { get; set; }
        public string AdminId { get; set; }

        // Navigation property For Orders
        public ICollection<Order>? Orders { get; set; } = new HashSet<Order>();
    }
}
