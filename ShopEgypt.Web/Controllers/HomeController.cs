namespace ShopEgypt.Web.Controllers
{
    public class HomeController(IProductRepository _productRepository) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Discover(int id,CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetDiscoverProductByIdAsync(id,cancellationToken);
            return View(product);
        }

    }
}
