//namespace ShopEgypt.Web.ViewComponents
//{
//    public class ShippedOrderViewComponent:ViewComponent
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        public ShippedOrderViewComponent(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            var numberOfOrders = _unitOfWork.OrderHeaderReposittory.GetAll(x=>x.OrderStatus==MyStatus.StatusShipped).Count();
//            return View("OrderCount", numberOfOrders);
//        }
//    }
//}
