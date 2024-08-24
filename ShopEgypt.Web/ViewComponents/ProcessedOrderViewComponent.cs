namespace ShopEgypt.Web.ViewComponents
{
    public class ProcessedOrderViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProcessedOrderViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var numberOfOrders = _unitOfWork.OrderHeaderReposittory.GetAll(x=>x.OrderStatus==MyStatus.StatusInProcess).Count();
            return View("OrderCount", numberOfOrders);
        }
    }
}
