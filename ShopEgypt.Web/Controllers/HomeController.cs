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
            var productsForDiscover = await _productRepository.GetAllProductsForDiscoverAsync();
            var productsForBuy = await _productRepository.GetAllProductsForBuyAsync();
            var categoryForDiscover=await _categoryRepository.GetAllCategoriesAsync();
            var response = new HomeViewVM
            {
                ProductsForDiscover = productsForDiscover.ToList(),
                productsForBuy = productsForBuy.ToList(),
                CategoriesResponse = categoryForDiscover.ToList()
            };
            return View(response);
        }


        [HttpGet]
        public async Task<IActionResult> Discover(int id,CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductForDiscoverByIdAsync(id,cancellationToken);
            return View(product);
        }

    }
}
