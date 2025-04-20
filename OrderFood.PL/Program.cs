using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFood.BLL.Interfaces;
using OrderFood.BLL.Repositories;
using OrderFood.DAL.Context;
using OrderFood.DAL.Data.DataSeed.Entities;
using OrderFood.DAL.Data.DataSeed.Identity;
using OrderFood.DAL.Data.DataSeed.Identity.Users;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Helper;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace OrderFood.PL;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<FoodDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<FoodDbContext>();

        builder.Services.AddControllersWithViews();

        // Add Unit Of Work To The Container
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IBasketRepository, BasketRepository>();

        builder.Services.AddScoped<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(new ConfigurationOptions
             {
                 EndPoints = { { "redis-10154.crce176.me-central-1-1.ec2.redns.redis-cloud.com", 10154 } },
                 User = "default",
                 Password = "NQYpHcpMhgm7mLBwnu3DZo4u8b9keOsC"
             });
        });



        // Add AutoMapper Service
        builder.Services.AddAutoMapper(typeof(MappingProfiles));



        var app = builder.Build();

        // Seed Data For Database

        using var scope = app.Services.CreateScope();

        var service = scope.ServiceProvider;

        var LoggerFactory = service.GetRequiredService<ILoggerFactory>();

        try
        {
            var DbContext = service.GetRequiredService<FoodDbContext>();

            await DbContext.Database.MigrateAsync();

            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            var dbContext = service.GetRequiredService<FoodDbContext>();

            await RoleSeed.RolesSeedAsync(roleManager);

            await IdentitySeed.SeedUserAsync(userManager);

            await EntityStoreSeed.SeedAsync(dbContext);

        }
        catch (Exception ex)
        {

            var Logger = LoggerFactory.CreateLogger<Program>();
            Logger.LogError(ex, "Error during update Database");
        }




        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();
        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapStaticAssets();
        app.MapAreaControllerRoute(
            name: "Identity",
            areaName: "Identity",
            pattern: "Identity/{controller=Home}/{action=OnboardingPage}/{id?}")
            .WithStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=OnboardingPage}/{id?}")
            .WithStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        app.Run();
    }
}
