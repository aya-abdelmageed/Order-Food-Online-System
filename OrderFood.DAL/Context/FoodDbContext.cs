using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderFood.DAL.Entities;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Context
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OrderMeals>()
                .HasKey(om => new { om.OrderId, om.MealId });

            // create NonClustered Index on Restaurant Name
            builder.Entity<Restaurant>()
                .HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("IX_Restaurant_Name");

            // create NonClustered Index on Meal Name
            builder.Entity<Meal>()
                .HasIndex(m => m.Name)
                .IsUnique()
                .HasDatabaseName("IX_Meal_Name");

            base.OnModelCreating(builder);
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<OrderMeals> MealOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
