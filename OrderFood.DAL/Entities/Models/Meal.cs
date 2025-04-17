using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class Meal : BaseEntity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public int SoldCount { get; set; } = 0;

        // Navigation Property For Category
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public int CategoryId { get; set; }

        // Navigation Property For OrderMeals
        public ICollection<OrderMeals>? OrderMeals { get; set; } = new HashSet<OrderMeals>();
    }
}
