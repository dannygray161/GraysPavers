using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GraysPavers_DataAccess.Data;
using GraysPavers_DataAccess.Repository.IRepository;
using GraysPavers_Models;
using GraysPavers_Models.ViewModels;
using GraysPavers_Utility;
using Newtonsoft.Json;

namespace GraysPavers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _prodRepo;
        private readonly ICategoryRepository _catRepo;
        public HomeController(ILogger<HomeController> logger, IProductRepository prodRepo, ICategoryRepository catRepo)

        {
            _logger = logger;
            _prodRepo = prodRepo;
            _catRepo = catRepo;
        }

        public IActionResult Index()
        {
            HomePageViewModel homeVM = new HomePageViewModel
            {
                Products = _prodRepo.GetAll(includeProperties:"Category,AppType"),
                Categories = _catRepo.GetAll()
            };

            return View(homeVM);
        }
        public IActionResult Index1()
        {
           

            return View(nameof(Index1));
        }

        public IActionResult ChatNotLoggedIn()
        {
            TempData[WebConstants.Error] = "Please Sign In";
            

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingcartlist = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingcartlist = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }


            DetailsViewModel DetailsViewModel = new DetailsViewModel()
            {
                Product = _prodRepo.FirstOrDefault(u => u.Id == id, includeProperties: "Category,AppType"),
                    
                ExistsInCart = false

                /* here, we initialize detailsVM to a new instance, then we set product equal to _db.product (to retrieve from the database)
                 but we wish to include category and apptype, so we eager load them. then we want this product where the id passed is equal 
                to the product id in the database, and we have to use first or default because where cannot return more than one object
                so we trick the code into getting the first or default where both includes are loaded. We then set our bool, exists in cart to false 
                for now, then we return the view of detailsViewModel completing this action method.
                
                 then we need to check session and check if this product exists in the session
                or not, then we need to set the boolean flag for existsincart*/

            };
            foreach (var item in shoppingcartlist)
            {
                if (item.Id == id)
                {
                    // item exists in cart so we have to set the flag to true
                    DetailsViewModel.ExistsInCart = true;
                }
            }
            return View(DetailsViewModel);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsViewModel detailsViewModel)
        {
            //see txt file for explanation

            List<ShoppingCart> shoppingcartlist = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingcartlist = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            shoppingcartlist.Add(new ShoppingCart { Id = id, SqFt = detailsViewModel.Product.TempSqFt});
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingcartlist);
            TempData[WebConstants.Success] = "Added To Cart";
            return RedirectToAction(nameof(Index));

        }
        public IActionResult RemoveFromCart(int id)
        {
            //see txt file for explanation

            List<ShoppingCart> shoppingcartlist = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                shoppingcartlist = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var itemToRemove = shoppingcartlist.SingleOrDefault(i => i.Id == id);
            if (itemToRemove != null)
            {
                shoppingcartlist.Remove(itemToRemove);
                TempData[WebConstants.Success] = "Item Removed";
            }


            HttpContext.Session.Set(WebConstants.SessionCart, shoppingcartlist);
            return RedirectToAction(nameof(Index));

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


    }
}
