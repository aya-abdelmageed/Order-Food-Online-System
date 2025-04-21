using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFood.PL.Areas.Resturant.ViewModel
{
    public class CouponViewModel
    {
        [Required]
        public int AmountPercentage { get; set; }
        public string Code { get; set; }

        public IFormFile? ImageFile { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }

       

    }
}
