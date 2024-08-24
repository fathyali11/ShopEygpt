namespace ShopEgypt.Web.ViewComponents
{
    public class CancellededOrderViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public CancellededOrderViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var numberOfOrders = _unitOfWork.OrderHeaderReposittory.GetAll(x=>x.OrderStatus==MyStatus.StatusCancelled).Count();
            return View("OrderCount", numberOfOrders);
        }
    }
}
