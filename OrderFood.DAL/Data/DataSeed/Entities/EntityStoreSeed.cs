using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OrderFood.DAL.Data.DataSeed.Entities
{
    public static class EntityStoreSeed
    {
        public static async Task SeedAsync(FoodDbContext dbContext)
        {
            var staticPath = "../OrderFood.DAL/Data/DataSeed/Entities";

            var RestaurantPath = $"{staticPath}/Restaurant.json";
            await TransferData<Restaurant>(RestaurantPath, dbContext);

            var ReviewPath = $"{staticPath}/Review.json";
            await TransferData<Review>(ReviewPath, dbContext);

            var CategoryPath = $"{staticPath}/Category.json";
            await TransferData<Category>(CategoryPath, dbContext);

            // Add SQL Query for CategoryRestaurant
            AddSqlQuery(dbContext);

            var MealsPath = $"{staticPath}/Meals.json";
            await TransferData<Meal>(MealsPath, dbContext);


            var CouponsPath = $"{staticPath}/Coupons.json";
            await TransferData<Coupon>(CouponsPath, dbContext);

            var OrderPath = $"{staticPath}/Order.json";
            await TransferData<Order>(OrderPath, dbContext);

            var OrderMealsPath = $"{staticPath}/OrderMeals.json";
            await TransferData<OrderMeals>(OrderMealsPath, dbContext);

        }

        private static async Task TransferData<T>(string DataPath, FoodDbContext dbContext) where T : BaseEntity
        {
            if (!dbContext.Set<T>().Any())
            {
                var ItemsData = File.ReadAllText(DataPath);
                var Items = JsonSerializer.Deserialize<List<T>>(ItemsData);
                if (Items?.Count > 0)
                {
                    foreach (var Item in Items)
                    {
                        await dbContext.Set<T>().AddAsync(Item);
                    }
                    var result = await dbContext.SaveChangesAsync();
                }
            }
        }


        private static void AddSqlQuery(FoodDbContext dbContext)
        {

            var hasData = dbContext.Set<Category>()
                            .SelectMany(c => c.Restaurants)
                            .Any();

            if (!hasData)
            {
                dbContext.Database.ExecuteSqlRaw(@"INSERT INTO CategoryRestaurant (RestaurantsId, CategoriesId)
                                                VALUES
                                                (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), 
                                                (2, 1), (2, 2), (2, 3), (2, 4), (2, 5), 
                                                (3, 1), (3, 2), (3, 3), (3, 4), (3, 5), 
                                                (4, 1), (4, 2), (4, 3), (4, 4), (4, 5), 
                                                (5, 1), (5, 2), (5, 3), (5, 4), (5, 5);");
            }

        }
    }
}
