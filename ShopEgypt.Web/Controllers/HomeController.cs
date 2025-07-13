using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ShopEgypt.Web.Controllers
{
    public class HomeController(IProductRepository _productRepository) : Controller
    {
        //public IActionResult Index(string searchItem)
        //{

        //    var products = _unitOfWork.ProductRepository.GetAll(includeObj: "Category");
        //    if (!string.IsNullOrEmpty(searchItem))
        //    {
        //        products = _unitOfWork.ProductRepository.GetAll(x => x.Name.ToLower() == searchItem.ToLower() || x.Category.Name == searchItem, includeObj: "Category");
        //        ViewBag.CurrentFilter = searchItem;
        //    }


        //    return View(products);
        //}
        //public IActionResult DiplayProducts(int categoryId)
        //{
        //    var products = _unitOfWork.ProductRepository.GetAll(x => x.CategoryId == categoryId, includeObj: "Category");
        //    return View(nameof(Index), products);
        //}
        //public IActionResult NewProducts()
        //{
        //    var products = _unitOfWork.ProductRepository.GetAll(includeObj: "Category").Take(5);
        //    return View(nameof(Index), products);
        //}
        //public IActionResult DisplayCategoies(string searchItem)
        //{
        //    var categories = _unitOfWork.CategoryRepository.GetAll();
        //    if (!string.IsNullOrEmpty(searchItem))
        //    {
        //        categories = _unitOfWork.CategoryRepository.GetAll(x => x.Name.ToLower() == searchItem.ToLower());
        //        ViewBag.CurrentFilter = searchItem;
        //    }
        //    return View(categories);
        //}
        //public IActionResult Details(int id)
        //{
        //    var claims = (ClaimsIdentity)User.Identity;
        //    var userID = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    ShoppingCart? cartFromDb = _unitOfWork.ShoppingCartRepository.GetBy(x => x.ProductId == id && x.UserId == userID);

        //    var cart = new ShoppingCart()
        //    {
        //        Product = _unitOfWork.ProductRepository.GetBy(x => x.Id == id, includeObj: "Category"),
        //        UserId = userID,
        //        ProductId = id
        //    };
        //    if (cartFromDb != null)
        //        cart.Count = cartFromDb.Count;
        //    return View(cart);
        //}

        [HttpGet]
        public async Task<IActionResult> Discover(int id,CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductForDiscoverByIdAsync(id,cancellationToken);
            return View(product);
        }

    }
}
