using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Data.DataSeed.Identity
{
    public static class RoleSeed
    {
        public static async Task RolesSeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("Owner"));
                await roleManager.CreateAsync(new IdentityRole("Delivery"));
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }
        }
    }
}
