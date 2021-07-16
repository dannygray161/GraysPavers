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
                InquiryHeader = _headerRepo.FirstOrDefault(u => u.InquiryId == id),
                InquiryDetails = _detailsRepo.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product")

            };
            return View(InquiryViewModel);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new {data = _headerRepo.GetAll()});
        }

        #endregion
















    }


}

