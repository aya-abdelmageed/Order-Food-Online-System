using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;

namespace OrderFood.PL.Models
{
    public class RestaurantSettingsViewModel
    {
        //public File LogoFile { get; set; }
        public Restaurant Restaurant { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
