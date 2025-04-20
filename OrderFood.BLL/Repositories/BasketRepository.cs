using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        public Task<bool> DeleteBasketAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerBasket?> GetBasketAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket)
        {
            throw new NotImplementedException();
        }
    }
}
