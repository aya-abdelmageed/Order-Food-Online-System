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

        //--Google Authintication

        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
               options.ClientId = "99950342398-k1lib9ust9o852cfnoirnf4f6t3u7nkb.apps.googleusercontent.com";
               options.ClientSecret = "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";
            });
        // Add Unit Of Work To The Container
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

        app.MapAreaControllerRoute(
   name: "area",
   areaName: "Resturant",
   pattern: "{controller=Restaurants}/{action=GetMenu}/{id=4}");

        app.MapStaticAssets();
        app.MapAreaControllerRoute(
            name: "Identity",
            areaName:"Identity",
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
