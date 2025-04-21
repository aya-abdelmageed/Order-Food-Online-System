using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderFood.DAL.Entities;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
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


            // drop Id and IsDeleted column from OrderMeals
            builder.Entity<OrderMeals>()
                .Ignore(om => om.Id);
            

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


            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Only apply this rule to entities that have IsDeleted
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");               // e =>
                    var isDeletedProperty = Expression.Property(parameter, nameof(BaseEntity.IsDelete));         // e.IsDeleted
                    var notDeleted = Expression.Not(isDeletedProperty);                          // !e.IsDeleted
                    var lambda = Expression.Lambda(notDeleted, parameter);                       // e => !e.IsDeleted

                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);              // Apply the filter
                }
            }

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
