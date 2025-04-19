using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;

namespace OrderFood.PL.Areas.Resturant.ViewModel
{
    public class UploadViewModel
    {
        public IFormFile ImageFile { get; set; }
        public Restaurant Restaurant { get; set; }
        
    }
}
