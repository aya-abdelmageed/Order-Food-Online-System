using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Basket
{
    public class CustomerFavourite : BaseBasket
    {
        public CustomerFavourite()
        {
            
        }
        public CustomerFavourite(string id)
        {
            Id = id;
        }

        public List<FavouriteItem> FavouriteItems { get; set; } = new List<FavouriteItem>();
    }
}
