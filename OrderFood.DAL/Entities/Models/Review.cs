using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class Review : BaseEntity
    {
        public DateTime ReviewDate { get; set; } = DateTime.Now;
        public int Rate { get; set; }
        public string? Comment { get; set; }

        // Navigation properties For Customer
        [ForeignKey("CustomerId")]
        public ApplicationUser? Customer { get; set; }
        public string CustomerId { get; set; }

        // Navigation properties For Restaurant
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }
        public int RestaurantId { get; set; }
    }
}
