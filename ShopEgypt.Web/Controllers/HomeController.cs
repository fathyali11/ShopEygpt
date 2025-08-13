namespace ShopEgypt.Web.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TooManyRequests([FromQuery]int retryAfterSeconds)
    {
        return View(retryAfterSeconds);
    }

}
