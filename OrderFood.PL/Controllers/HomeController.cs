using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.PL.Models;

namespace OrderFood.PL.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _context;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork context)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult OnboardingPage()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    //----------------------------------------------------------------------------
    public async Task<IActionResult> GetMostPopularMeals()
    {
        var meals = await _context.GetRepository<Meal>()
            .GetAllAsync(m => !m.IsDelete);
          

        var mostPopularMeals = meals
            .OrderByDescending(m => m.SoldCount) 
            .Take(12) 
            .ToList();

        //ViewBag.MostPopularMeals = mostPopularMeals;
        return PartialView("_MostPopular", mostPopularMeals);
   
    }
    //------------------------------------------------------------------------
    public async Task<IActionResult> ViewValidCoupons()
    {
        var coupons = await _context.GetRepository<Coupon>().GetAllAsync();

        var validCoupons = coupons
            .Where(c => c.ExpireDate >= DateTime.Now) 
            .OrderBy(c => c.ExpireDate)
            .ToList();

        return PartialView("_CouponsView", validCoupons);
    }


}
