using OrderFood.DAL.Entities.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Interfaces
{
    public interface IBasketRepository<T> where T : BaseBasket, new()
    {
        public Task<bool> DeleteBasketAsync(string id);

        public Task<T?> GetBasketAsync(string id);

        public Task<T?> UpdateBasketAsync(T Basket);
    }
}
