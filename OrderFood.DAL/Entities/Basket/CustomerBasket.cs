using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Basket
{
    public class CustomerBasket : BaseBasket
    {
        public CustomerBasket()
        {
            
        }
        public CustomerBasket(string id)
        {
            Id = id;
        }
        public List<BasketItem> basketItems { get; set; } = new List<BasketItem>();
    }
}
