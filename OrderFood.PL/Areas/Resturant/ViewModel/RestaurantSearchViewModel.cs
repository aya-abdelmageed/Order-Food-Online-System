using OrderFood.DAL.Entities.Models;

namespace OrderFood.PL.Areas.Resturant.ViewModel
{
    public class RestaurantSearchViewModel
    {
        public string NameSearch { get; set; }
        public string OwnerSearch { get; set; }
        public string AddressSearch { get; set; }
        public int PageNo { get; set; } = 1;
        public int NoOfPages { get; set; }
        public List<Restaurant> Restaurants { get; set; }
    }
}
