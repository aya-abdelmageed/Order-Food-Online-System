using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFood.PL.Areas.Delivery.ViewModel
{
    public class RestaurantVM
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string HotLine { get; set; }
        public string Logo { get; set; }
        public double? Long { get; set; }
        public double? Lat { get; set; }

    }
}
