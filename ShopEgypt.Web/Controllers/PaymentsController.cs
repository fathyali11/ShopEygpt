using Microsoft.AspNetCore.Mvc;

namespace ShopEgypt.Web.Controllers;
public class PaymentsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
