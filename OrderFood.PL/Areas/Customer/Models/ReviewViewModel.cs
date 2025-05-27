using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFood.PL.Areas.Customer.Models
{
    public class ReviewViewModel
    {
        public int RestaurantId { get; set; }
        public int Rate { get; set; }
        public string? Comment { get; set; }
       
    }
}
