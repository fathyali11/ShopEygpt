using Microsoft.AspNetCore.Authorization;

namespace ShopEgypt.Web.Areas.Customer.Controllers
{
    [Area(SD.CustomerRole)]
    
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products=_unitOfWork.ProductRepository.GetAll(includeObj:"Category").ToList();
            return View(products);
        }
        public IActionResult Details(int id)
        {
            var product=_unitOfWork.ProductRepository.GetBy(x=>x.Id == id,includeObj:"Category");
            return View(product);
        }
    }
}
