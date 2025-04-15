using Microsoft.AspNetCore.Identity;
using OrderFood.DAL.Context;
using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderFood.DAL.Data.DataSeed.Identity.Users
{
    public static class IdentitySeed
    {
        public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var staticPath = "../OrderFood.DAL/Data/DataSeed/Identity/Users";

                var AdminJson = File.ReadAllText($"{staticPath}/Admin.json");
                List<ApplicationUser> Admin = JsonSerializer.Deserialize<List<ApplicationUser>>(AdminJson)!;
                await TransferData(Admin, "Admin", userManager);
                
                var CustomerJson = File.ReadAllText($"{staticPath}/Customer.json");
                List<ApplicationUser> Customer = JsonSerializer.Deserialize<List<ApplicationUser>>(CustomerJson)!;
                await TransferData(Customer, "Customer", userManager);

                var OwnerJson = File.ReadAllText($"{staticPath}/Owner.json");
                List<ApplicationUser> Owner = JsonSerializer.Deserialize<List<ApplicationUser>>(OwnerJson)!;
                await TransferData(Owner, "Owner", userManager);
                
                var DeliveryJson = File.ReadAllText($"{staticPath}/Delivery.json");
                List<ApplicationUser> Delivery = JsonSerializer.Deserialize<List<ApplicationUser>>(DeliveryJson)!;
                await TransferData(Delivery, "Delivery", userManager);
            }
        }

        private static async Task TransferData(List<ApplicationUser> users, string Role, UserManager<ApplicationUser> userManager)
        {
            foreach (var user in users)
            {
                if(user.Email != null)
                    user.UserName = user.Email.Split('@')[0];
                else
                    user.UserName = "User" + Guid.NewGuid().ToString().Substring(0, 5);
                var createUserResult = await userManager.CreateAsync(user, "P@$$w0rd");
                if (createUserResult.Succeeded)
                    await userManager.AddToRoleAsync(user, Role);

            }
        }

    }
}
