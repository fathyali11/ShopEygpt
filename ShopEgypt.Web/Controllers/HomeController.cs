using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Entites.ViewModels.HomeVMs;


namespace ShopEgypt.Web.Controllers
{
    public class HomeController(IProductRepository _productRepository,
        ICategoryRepository _categoryRepository) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var response = new HomeViewVM
            {
                ProductsForDiscover = await _productRepository.GetDiscoverProductsAsync(cancellationToken),
                NewArrivalProducts      = await _productRepository.GetNewArrivalProductsAsync(cancellationToken),
                CategoriesResponse = await _categoryRepository.GetAllCategoriesAsync(),
                BestSellingProducts=await _productRepository.GetBestSellingProductsAsync(cancellationToken)
            };
            return View(response);
        }


        [HttpGet]
        public async Task<IActionResult> Discover(int id,CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetDiscoverProductByIdAsync(id,cancellationToken);
            return View(product);
        }

    }
}
