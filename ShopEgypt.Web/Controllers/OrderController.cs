using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Web.Entites.ViewModels;

namespace ShopEgypt.Web.Controllers
{
    [Area(SD.AdminRole)]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetData()
        {
            var OrdderHeaders = _unitOfWork.OrderHeaderReposittory.GetAll();
            return Json(new { data= OrdderHeaders });
        }
        public IActionResult Details(int  OrderId)
        {
            var OrderVM = new OrderVM
            {
                OrderHeader = _unitOfWork.OrderHeaderReposittory.GetBy(x => x.Id == OrderId),
                OrderDetail=_unitOfWork.OrderDetailReposittory.GetAll(x=>x.OrderHeaderId== OrderId, includeObj:"Product")
            };
            return View(OrderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Details))]
        public IActionResult UpdateOrderDetails()
        {
            var OrderFromDB=_unitOfWork.OrderHeaderReposittory.GetBy(x=>x.Id==OrderVM.OrderHeader.Id);
            OrderFromDB.Name=OrderVM.OrderHeader.Name;
            OrderFromDB.Email=OrderVM.OrderHeader.Email;
            OrderFromDB.PhoneNumber=OrderVM.OrderHeader.PhoneNumber;
            OrderFromDB.City=OrderVM.OrderHeader.City;
            if(OrderVM.OrderHeader.Carrier is not null)
                OrderFromDB.Carrier=OrderVM.OrderHeader.Carrier;
            if(OrderVM.OrderHeader.TrackNumber is not null)
                OrderFromDB.TrackNumber=OrderVM.OrderHeader.TrackNumber;

            _unitOfWork.OrderHeaderReposittory.Update(OrderFromDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { OrderId = OrderFromDB.Id});
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
			var OrderFromDB = _unitOfWork.OrderHeaderReposittory.GetBy(x => x.Id == OrderVM.OrderHeader.Id);
			_unitOfWork.OrderHeaderReposittory.UpdateStatus(OrderFromDB.Id, MyStatus.StatusInProcess, null);
			_unitOfWork.OrderHeaderReposittory.Update(OrderFromDB);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Details), new { OrderId = OrderFromDB.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShipping()
		{
			var OrderFromDB = _unitOfWork.OrderHeaderReposittory.GetBy(x => x.Id == OrderVM.OrderHeader.Id);
			_unitOfWork.OrderHeaderReposittory.UpdateStatus(OrderFromDB.Id, MyStatus.StatusShipped, null);
            OrderFromDB.Carrier=OrderVM.OrderHeader.Carrier;
            OrderFromDB.TrackNumber=OrderVM.OrderHeader.TrackNumber;
            OrderFromDB.ShippingDate=DateTime.Now;
			_unitOfWork.OrderHeaderReposittory.Update(OrderFromDB);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Details), new { OrderId = OrderFromDB.Id });
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
			var OrderFromDB = _unitOfWork.OrderHeaderReposittory.GetBy(x => x.Id == OrderVM.OrderHeader.Id);
            if(OrderFromDB.PaymentStatus==MyStatus.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = OrderFromDB.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund=service.Create(options);
                _unitOfWork.OrderHeaderReposittory.UpdateStatus(OrderFromDB.Id, MyStatus.StatusCancelled, MyStatus.StatusRefunded);


			}
            else
				_unitOfWork.OrderHeaderReposittory.UpdateStatus(OrderFromDB.Id, MyStatus.StatusCancelled, MyStatus.StatusCancelled);
			_unitOfWork.OrderHeaderReposittory.Update(OrderFromDB);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Details), new { OrderId = OrderFromDB.Id });
		}
        [HttpGet]
        public IActionResult GetProcessedOrder()
        {
            var ProcessedOrders=_unitOfWork
                .OrderHeaderReposittory
                .GetAll(x=>x.OrderStatus==MyStatus.StatusInProcess);
            return Json(new {data=ProcessedOrders});
        }
        [HttpGet]
        public IActionResult ProcessedOrder()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetShippededOrder()
        {
            var ShippededOrders = _unitOfWork
                .OrderHeaderReposittory
                .GetAll(x => x.OrderStatus == MyStatus.StatusShipped);
            return Json(new { data = ShippededOrders });
        }
        [HttpGet]
        public IActionResult ShippededOrder()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCancelledOrders()
        {
            var ProcessedOrders = _unitOfWork
                .OrderHeaderReposittory
                .GetAll(x => x.OrderStatus == MyStatus.StatusCancelled);
            return Json(new { data = ProcessedOrders });
        }
        [HttpGet]
        public IActionResult CancelledOrders()
        {
            return View();
        }

    }
}
