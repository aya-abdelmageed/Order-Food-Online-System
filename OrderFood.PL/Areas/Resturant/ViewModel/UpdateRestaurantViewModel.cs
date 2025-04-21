using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System.ComponentModel.DataAnnotations;

namespace OrderFood.PL.Areas.Resturant.ViewModel
{
    public class UpdateRestaurantViewModel
    {
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(550, ErrorMessage = "Description cannot exceed 300 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "HotLine is required")]
        [RegularExpression(@"^\d{3,15}$", ErrorMessage = "HotLine must be between 3 and 15 digits")]
        public string HotLine { get; set; }

        //[Required(ErrorMessage = "Longitude is required")]
        public double? Long { get; set; }

        //[Required(ErrorMessage = "Latitude is required")]
        public double? Lat { get; set; }

        [Required]
        public Restaurant Restaurant { get; set; }
    }
}
