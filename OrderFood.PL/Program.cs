using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.Repositories;
using OrderFood.DAL.Context;
using OrderFood.DAL.Data.DataSeed.Entities;
using OrderFood.DAL.Data.DataSeed.Identity;
using OrderFood.DAL.Data.DataSeed.Identity.Users;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Helper;
using OrderFood.Service;
using StackExchange.Redis;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Threading.Tasks;


namespace OrderFood.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure database context
            builder.Services.AddDbContext<FoodDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Configure Identity
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<FoodDbContext>();

            builder.Services.AddControllersWithViews();

            // Google Authentication
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "99950342398-k1lib9ust9o852cfnoirnf4f6t3u7nkb.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                });

            // Register business services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped(typeof(IBasketRepository<>), typeof(BasketRepository<>));

            // Configure Redis connection
            builder.Services.AddScoped<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { { builder.Configuration.GetConnectionString("RedisConnection")!, int.Parse(builder.Configuration.GetConnectionString("RedisPort")!) } },
                    User = builder.Configuration.GetConnectionString("RedisUserName"),
                    Password = builder.Configuration.GetConnectionString("RedisPassword")
                });
            });

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfiles));

            // ? Register EmailSender service
            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            var app = builder.Build();

            // ? Seed roles, users, and data
            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                var dbContext = service.GetRequiredService<FoodDbContext>();
                await dbContext.Database.MigrateAsync();

                var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

                await RoleSeed.RolesSeedAsync(roleManager);
                await IdentitySeed.SeedUserAsync(userManager);
                await EntityStoreSeed.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error during database update or seeding.");
            }

            // Configure HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Area routing
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapAreaControllerRoute(
                name: "Identity",
                areaName: "Identity",
                pattern: "Identity/{controller=Home}/{action=OnboardingPage}/{id?}")
                .WithStaticAssets();

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=OnboardingPage}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages().WithStaticAssets();

            app.Run();
        }
    }
}
