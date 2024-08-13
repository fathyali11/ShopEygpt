using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using Web.Entites.Models;
using X.PagedList;
using X.PagedList.Extensions;


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
            var products = _unitOfWork.ProductRepository.GetAll(includeObj: "Category");
            return View(products);
        }
        public IActionResult DiplayProducts(int categoryId,int ?page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var products = _unitOfWork.ProductRepository.GetAll(x=>x.CategoryId==categoryId,includeObj: "Category").ToPagedList(pageNumber,pageSize);
            return View(products);
        }
        public IActionResult DisplayCategoies()
        {
            var categories = _unitOfWork.CategoryRepository.GetAll();
            return View(categories);
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
