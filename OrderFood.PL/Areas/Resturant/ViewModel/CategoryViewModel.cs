using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OrderFood.PL.Areas.Resturant.ViewModel
{
    public class CategoryViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int RestaurantId { get; set; }


        [Required(ErrorMessage = "Image file is required.")]
        public IFormFile ImageFile { get; set; }  // Accept uploaded image
    }
}
