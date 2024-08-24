namespace ShopEgypt.Web.ViewComponents
{
    public class OrderViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var numberOfOrders=_unitOfWork.OrderHeaderReposittory.GetAll().Count();
            return View("OrderCount", numberOfOrders);
        }
    }
}
