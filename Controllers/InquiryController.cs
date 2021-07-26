using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using GraysPavers_DataAccess.Data;
using GraysPavers_DataAccess.Repository.IRepository;
using GraysPavers_Models;
using GraysPavers_Models.ViewModels;
using GraysPavers_Utility;

namespace GraysPavers.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]



    public class InquiryController : Controller
    {

        private readonly IInquiryHeaderRepository _headerRepo;
        private readonly IInquiryDetailsRepository _detailsRepo;

        [BindProperty]
        public InquiryViewModel InquiryViewModel { get; set; }

        public InquiryController(IInquiryHeaderRepository headerRepo, IInquiryDetailsRepository detailsRepo)
        {
            _detailsRepo = detailsRepo;
            _headerRepo = headerRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            InquiryViewModel = new InquiryViewModel()
            {
                InquiryHeader = _headerRepo.FirstOrDefault(u => u.Id == id),
                InquiryDetails = _detailsRepo.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product")

            };
            return View(InquiryViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            InquiryViewModel.InquiryDetails =
                _detailsRepo.GetAll(u => u.InquiryHeaderId == InquiryViewModel.InquiryHeader.Id);


            foreach (var detail in InquiryViewModel.InquiryDetails)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    Id = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WebConstants.SessionInquiryId, InquiryViewModel.InquiryHeader.Id);

            return RedirectToAction("Index", "ShoppingCart");
        }



        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _headerRepo.FirstOrDefault(u => u.Id == InquiryViewModel.InquiryHeader.Id);
            IEnumerable<InquiryDetails> inquiryDetails =
                _detailsRepo.GetAll(u => u.InquiryHeaderId == InquiryViewModel.InquiryHeader.Id);

            _detailsRepo.RemoveRange(inquiryDetails);
            _headerRepo.Remove(inquiryHeader);
            _headerRepo.Save();



            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _headerRepo.GetAll() });
        }

        #endregion
















    }


}

