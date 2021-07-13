using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using GraysPavers_DataAccess.Data;
using GraysPavers_Models;
using GraysPavers_Models.ViewModels;
using GraysPavers_Utility;

namespace GraysPavers.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ProductUserViewModel ProductUserViewModel { get; set; }
        
        public ShoppingCartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null && 
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                // if we make it here that means session exists and we can retreive a list of products and display them

                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);




            }

            // selecting prod id and storing in a list of int, then creating a list of prod and comparing the ID we retrieved with existing ID's in prodInCart
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(u => prodInCart.Contains(u.ProductId));


            return View(productList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                // if we make it here that means session exists and we can retreive a list of products and display them

                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);

            }

            // selecting prod id and storing in a list of int, then creating a list of prod and comparing the ID we retrieved with existing ID's in prodInCart
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(u => prodInCart.Contains(u.ProductId));
            ProductUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault((u => u.Id == claim.Value)),
                ProductList = productList.ToList()
            };

            return View(ProductUserViewModel);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserViewModel productUserViewModel)
        {

            var pathtotemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() +
                                 "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
            var subject = "New Inquiry";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(pathtotemplate))
            {
                HtmlBody = sr.ReadToEnd();

            }

            StringBuilder productListSB = new StringBuilder();

            foreach (var prod in productUserViewModel.ProductList)
            {
                productListSB.Append(
                    $"Name: {prod.ProductName} <span style='font-size:14px;'>(ID:{prod.ProductId})</span><br/>");
            }

            var fullName = productUserViewModel.ApplicationUser.FirstName + " " +
                           productUserViewModel.ApplicationUser.LastName;
            string messageBody = string.Format(HtmlBody, fullName, productUserViewModel.ApplicationUser.Email,
                productUserViewModel.ApplicationUser.PhoneNumber, productListSB.ToString());


            //await _emailSender.SendEmailAsync(WebConstants.AdminEmail, subject, messageBody);


            return RedirectToAction(nameof(InquiryConfirmation));

        }
        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();

            return View();

        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
            {
                // if we make it here that means session exists and we can retreive a list of products and display them

                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);

            }
            // then we need to remove products based on product id == id
            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));

            // then we set the session again with our new shoppingCartList, and return to the Index view with redirect to action
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));

        }
    }
}
