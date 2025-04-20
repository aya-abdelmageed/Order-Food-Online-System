using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities.Basket
{
    public class CustomerBasket
    {

        public CustomerBasket(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
        public List<BasketItem> Items { get; set; }

        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }
    }
}
