using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; } 
        public string HotLine { get; set; }
        public string Logo { get; set; }
        public double? Long { get; set; }
        public double? Lat { get; set; }

        // Navigation Properties For User Owner
        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }
        public string OwnerId { get; set; }

        // Navigation Properties For Categories
        public ICollection<Category>? Categories { get; set; } = new HashSet<Category>();

        // Navigation Properties For OrderMeals
        public ICollection<OrderMeals>? OrderMeals { get; set; } = new HashSet<OrderMeals>();
    }
}
