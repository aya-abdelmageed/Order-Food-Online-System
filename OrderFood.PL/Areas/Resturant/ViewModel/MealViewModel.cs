using System.ComponentModel.DataAnnotations;

namespace OrderFood.PL.Areas.Resturant.ViewModel
{
    public class MealViewModel
    {
        [Required]
        public string Name { get; set; }

        public IFormFile ImageFile { get; set; } 

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }



    }
}
