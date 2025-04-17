using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Models
{
    public class OrderMeals : BaseEntity
    {
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public int OrderId { get; set; }

        [ForeignKey("MealId")]
        public Meal? Meal { get; set; }
        public int MealId { get; set; }

        public int Quantity { get; set; } = 1;

    }
}
