using System.ComponentModel.DataAnnotations;

namespace OrderFood.PL.Areas.Resturant.ViewModel
{
    public class RestaurantViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string HotLine { get; set; }

        // Logo (file upload)
        [Required]
        public IFormFile Logo { get; set; }

        public double? Lat { get; set; }
        public double? Long { get; set; }


        public string OwnerId { get; set; }

    }
}