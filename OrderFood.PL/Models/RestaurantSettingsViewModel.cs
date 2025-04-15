using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;

namespace OrderFood.PL.Models
{
    public class RestaurantSettingsViewModel
    {
        public string ActiveTab { get; set; } = "restaurant";
        public Restaurant Restaurant { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
