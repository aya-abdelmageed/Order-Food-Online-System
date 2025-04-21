using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Basket;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderFood.BLL.Repositories
{
    public class BasketRepository<T> : IBasketRepository <T> where T : BaseBasket, new()
    {
        private readonly IDatabase database;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            database = redis.GetDatabase();
        }

        public Task<bool> DeleteBasketAsync(string id)
        {
            return database.KeyDeleteAsync(id);
        }

        public async Task<T?> GetBasketAsync(string id)
        {
            var basket = await database.StringGetAsync(id);

            return basket.IsNull ? null : JsonSerializer.Deserialize<T>(basket!);
        }

        public async Task<T?> UpdateBasketAsync(T Basket)
        {
            var JsonBasket = JsonSerializer.Serialize(Basket);
            var result = await database.StringSetAsync(Basket.Id, JsonBasket, TimeSpan.FromDays(3));

            return result ? await GetBasketAsync(Basket.Id) : null;
        }
    }
}
