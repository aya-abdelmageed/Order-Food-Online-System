using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Restaurant>? Restaurants { get; set; } = new HashSet<Restaurant>();
        public ICollection<Meal>? Meals { get; set; } = new HashSet<Meal>();

    }
}
