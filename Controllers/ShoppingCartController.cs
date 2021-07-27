using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using GraysPavers_DataAccess.Data;
using GraysPavers_DataAccess.Repository;
using GraysPavers_DataAccess.Repository.IRepository;
using GraysPavers_Models;
using GraysPavers_Models.ViewModels;
using GraysPavers_Utility;
using GraysPavers_Utility.BrainTree;

namespace GraysPavers.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {





        private readonly IProductRepository _prodRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IInquiryDetailsRepository _detailsRepo;
        private readonly IInquiryHeaderRepository _headerRepo;
        private readonly IOrderDetailsRepository _orderDetailsRepo;
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IBrainTreeGate _braintreeGate;


        [BindProperty]
        public ProductUserViewModel ProductUserViewModel { get; set; }
        
        public ShoppingCartController(IProductRepository 
                prodRepo,IApplicationUserRepository userRepo, 
            IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IInquiryHeaderRepository headerRepo,
            IInquiryDetailsRepository detailsRepo, IOrderDetailsRepository orderDetailsRepo , IOrderHeaderRepository orderHeaderRepo, IBrainTreeGate brainTreeGate )
        {
            _orderDetailsRepo = orderDetailsRepo;
            _orderHeaderRepo = orderHeaderRepo;
            _detailsRepo = detailsRepo;
            _headerRepo = headerRepo;
            _prodRepo = prodRepo;
            _userRepo = userRepo;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _braintreeGate = brainTreeGate;
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
            List<int> prodInCart = shoppingCartList.Select(i => i.Id).ToList();
            IEnumerable<Product> productListTemp = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));
            IList<Product> productList = new List<Product>();

            foreach (var shoppingCart in shoppingCartList)
            {
                Product prodTemp = productListTemp.FirstOrDefault(u => u.Id == shoppingCart.Id);
                prodTemp.TempSqFt = shoppingCart.SqFt;
                productList.Add(prodTemp);
            }


            return View(productList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    Id = prod.Id,
                    SqFt = prod.TempSqFt
                });
            }
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);


            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            
            
                ApplicationUser applicationUser;
                if (User.IsInRole(WebConstants.AdminRole))
                {
                    if (HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)
                    {
                        InquiryHeader inquiryHeader = _headerRepo.FirstOrDefault(u =>
                            u.Id == HttpContext.Session.Get<int>(WebConstants.AdminRole));

                        applicationUser = new ApplicationUser()
                        {

                            Email = inquiryHeader.Email,
                            FullName = inquiryHeader.FullName,
                            PhoneNumber = inquiryHeader.PhoneNumber


                        };
                    }
                    else
                    {
                        applicationUser = new ApplicationUser();
                    }

                    //generate token 
                    var gateWay = _braintreeGate.GetGateway();
                    var clientToken = gateWay.ClientToken.Generate();
                    ViewBag.ClientToken = clientToken;










                }
                else
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    applicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value);

                }

                List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

                if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null &&
                    HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Count() > 0)
                {
                    // if we make it here that means session exists and we can retreive a list of products and display them

                    shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);

                }

                // selecting prod id and storing in a list of int, then creating a list of prod and comparing the ID we retrieved with existing ID's in prodInCart
                List<int> prodInCart = shoppingCartList.Select(i => i.Id).ToList();
                IEnumerable<Product> productList = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));
                ProductUserViewModel = new ProductUserViewModel()
                {
                    ApplicationUser = applicationUser
                };

                foreach (var obj in shoppingCartList)
                {
                    Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id == obj.Id);
                    prodTemp.TempSqFt = obj.SqFt;
                    ProductUserViewModel.ProductList.Add(prodTemp);
                }

                return View(ProductUserViewModel);

            
            

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserViewModel productUserViewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WebConstants.AdminRole))
            {
                //create order
                var address = ProductUserViewModel.ApplicationUser.AddressLine1 + " " +
                              ProductUserViewModel.ApplicationUser.AddressLine2 + " ";
                var fullName = ProductUserViewModel.ApplicationUser.FirstName + " " +
                               ProductUserViewModel.ApplicationUser.LastName + " ";
                var orderTotal = 0.0;
                foreach (var prod in ProductUserViewModel.ProductList)
                {
                    orderTotal += prod.ProductPrice * prod.TempSqFt;

                }

                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = orderTotal,
                    StreetAddress = address,
                    City = ProductUserViewModel.ApplicationUser.City,
                    State = ProductUserViewModel.ApplicationUser.State,
                    PostalCode = ProductUserViewModel.ApplicationUser.ZipCode,
                    FullName = fullName,
                    Email = ProductUserViewModel.ApplicationUser.Email,
                    PhoneNumber = ProductUserViewModel.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WebConstants.StatusPending


                };
                _orderHeaderRepo.Add(orderHeader);
                _orderHeaderRepo.Save();

                foreach (var prod in ProductUserViewModel.ProductList)
                {
                    OrderDetails orderDetail = new OrderDetails()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerSqFt = prod.ProductPrice,
                        Sqft = prod.TempSqFt,
                        ProductId = prod.Id
                    };
                    _orderDetailsRepo.Add(orderDetail);
                }
                _orderDetailsRepo.Save();


                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };
                var gateWay = _braintreeGate.GetGateway();
                Result<Transaction> result = await gateWay.Transaction.SaleAsync(request);
                try
                {
                    if (result.Target.ProcessorResponseText == "Approved")
                    {
                        orderHeader.TransactionId = result.Target.Id;
                        orderHeader.OrderStatus = WebConstants.StatusApproved;
                    }
                    else
                    {
                        orderHeader.OrderStatus = WebConstants.StatusCancelled;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected Error, Please Try Again");
                }

                _headerRepo.Save();



                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });

            }
            else
            {
                //create inquiry

                if (productUserViewModel == null)
                {
                    TempData[WebConstants.Error] = "Error Please try again";

                }

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
                        $"Name: {prod.ProductName} <span style='font-size:14px;'>(ID:{prod.Id})</span><br/>");
                }

                var fullName = productUserViewModel.ApplicationUser.FirstName + " " +
                               productUserViewModel.ApplicationUser.LastName;
                string messageBody = string.Format(HtmlBody, fullName, productUserViewModel.ApplicationUser.Email,
                    productUserViewModel.ApplicationUser.PhoneNumber, productListSB.ToString());


                await _emailSender.SendEmailAsync(WebConstants.AdminEmail, subject, messageBody);

                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    AppUserId = claim.Value,
                    FullName = fullName,
                    Email = ProductUserViewModel.ApplicationUser.Email,
                    PhoneNumber = ProductUserViewModel.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now


                };

                _headerRepo.Add(inquiryHeader);
                _headerRepo.Save();

                foreach (var prod in ProductUserViewModel.ProductList)
                {
                    InquiryDetails inquiryDetails = new InquiryDetails()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = prod.Id
                    };

                    _detailsRepo.Add(inquiryDetails);
                }
                TempData[WebConstants.Success] = "Inquiry Submitted Successfully";

                _detailsRepo.Save();

            }

            return RedirectToAction(nameof(InquiryConfirmation));

        }
        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == id);
            HttpContext.Session.Clear();

            return View(orderHeader);

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
            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.Id == id));

            // then we set the session again with our new shoppingCartList, and return to the Index view with redirect to action
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));

        }
        public IActionResult Clear()
        {





            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    Id = prod.Id,
                    SqFt = prod.TempSqFt
                });
            }
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
