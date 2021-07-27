using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using GraysPavers_DataAccess.Repository.IRepository;
using GraysPavers_Models;
using GraysPavers_Models.ViewModels;
using GraysPavers_Utility;
using GraysPavers_Utility.BrainTree;
using Microsoft.AspNetCore.Authorization;

namespace GraysPavers.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class OrderController : Controller
    {

        private readonly IOrderDetailsRepository _orderDetailsRepo;
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IBrainTreeGate _braintreeGate;

        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }

        public OrderController(IOrderDetailsRepository orderDetailsRepo, IOrderHeaderRepository orderHeaderRepo, IBrainTreeGate brainTreeGate)
        {
            _orderDetailsRepo = orderDetailsRepo;
            _orderHeaderRepo = orderHeaderRepo;
            _braintreeGate = brainTreeGate;
        }




        public IActionResult Index(string searchName=null, string searchEmail=null, string searchPhoneNumber=null, string Status=null)
        {

            OrderListViewModel orderListViewModel = new OrderListViewModel()
            {
                OrderHList = _orderHeaderRepo.GetAll(),
                StatusList = WebConstants.listStatus.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };


            if (!string.IsNullOrEmpty(searchName))
            {
                orderListViewModel.OrderHList =
                    orderListViewModel.OrderHList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListViewModel.OrderHList =
                    orderListViewModel.OrderHList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhoneNumber))
            {
                orderListViewModel.OrderHList =
                    orderListViewModel.OrderHList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhoneNumber.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--" )
            {
                orderListViewModel.OrderHList =
                    orderListViewModel.OrderHList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }


            return View(orderListViewModel);
        }

        public IActionResult Details(int id)
        {
            OrderViewModel = new OrderViewModel
            {
                OrderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == id),
                OrderDetail = _orderDetailsRepo.GetAll(u => u.OrderHeaderId == id, includeProperties:"Product")
            };

            return View(OrderViewModel);
        }

        
        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id);
            orderHeader.OrderStatus = WebConstants.StatusInProcess;
            _orderHeaderRepo.Save();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id);
            orderHeader.OrderStatus = WebConstants.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHeaderRepo.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHeaderRepo.FirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id);
            orderHeaderFromDb.FullName = OrderViewModel.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
            orderHeaderFromDb.Email = OrderViewModel.OrderHeader.Email;
            orderHeaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderViewModel.OrderHeader.City;
            orderHeaderFromDb.State = OrderViewModel.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderViewModel.OrderHeader.PostalCode;
            _orderHeaderRepo.Save();
            TempData[WebConstants.Success] = "Details Updated Successfully";
            ViewBag.data = "Successfully Updated ";

            return RedirectToAction("Details", "Order", new {id=orderHeaderFromDb.Id});
        }
        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id);

            var gateway = _braintreeGate.GetGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);
            if (transaction.Status == TransactionStatus.AUTHORIZED ||
                transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                // no refund needed
                Result<Transaction> resultVoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                //refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }




            orderHeader.OrderStatus = WebConstants.StatusRefunded;
            _orderHeaderRepo.Save();

            return RedirectToAction(nameof(Index));
        }
    }
    
}
