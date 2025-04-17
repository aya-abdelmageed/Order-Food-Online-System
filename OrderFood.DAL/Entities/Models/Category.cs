using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        // Navigation Property For Restaurant
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }
        public int RestaurantId { get; set; }

        public string Image { get; set; }

        public ICollection<Meal>? Meals { get; set; } = new HashSet<Meal>();

    }
}
