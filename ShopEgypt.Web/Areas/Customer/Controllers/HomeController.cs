using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize]
        public IActionResult Details(int id)
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userID = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCart? cartFromDb = _unitOfWork.ShoppingCartRepository.GetBy(x => x.ProductId ==id && x.UserId == userID);

            var cart = new ShoppingCart()
            {
                Product= _unitOfWork.ProductRepository.GetBy(x => x.Id == id, includeObj: "Category"),
                UserId=userID,
                ProductId=id
            };
            if(cartFromDb != null) 
                cart.Count = cartFromDb.Count;
            return View(cart);
        }

    }
}
