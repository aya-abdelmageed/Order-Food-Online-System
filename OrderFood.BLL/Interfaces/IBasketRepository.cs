using OrderFood.DAL.Entities.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Interfaces
{
    public interface IBasketRepository
    {
        public Task<bool> DeleteBasketAsync(string id);

        public Task<CustomerBasket?> GetBasketAsync(string id);

        public Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket);
    }
}
