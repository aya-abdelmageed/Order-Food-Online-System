using OrderFood.DAL.Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFood.PL.Areas.Delivery.ViewModel
{
    public class OrderMealsVM
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }


}
