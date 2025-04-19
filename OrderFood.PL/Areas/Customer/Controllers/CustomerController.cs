using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.BLL.Interfaces;
using OrderFood.DAL.Entities.Models;
using OrderFood.PL.Areas.Customer.Models;

namespace OrderFood.PL.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerController : Controller
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        public CustomerController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
        // GET: CustomerController

        ////the customer main page
        //public async Task<ActionResult> Index(string? name)
        //{
        //  //get the categories data to view
        //    var categories = await UnitOfWork.GetRepository<Category>().GetAllAsync(includes: m => m.Include(t => t.Meals));
        //    //get the distinct categories name
        //    var distinctCategories = categories.GroupBy(c => c.Name).Select(g => new CategoryViewModel()
        //    {
        //        CategoryName = g.Key,
        //        Image = g.First().Meals.OrderBy(i => i.Id).FirstOrDefault().Image,
        //    }
        //    ).ToList();
        //      //get restarant info to view
        //    var restaurants = await UnitOfWork.GetRepository<Restaurant>().GetAllAsync(
        //        includes: i => i.Include(c => c.Categories));

        //      //
        //    if (name != null)
        //    {
        //        restaurants = restaurants.Where(r => r.Categories.Any(c => c.Name == name)).ToList();
        //    }
        //    ViewData["Restaurants"] = restaurants;

        //    return View(distinctCategories);
        //}

        
    }
}
