using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShopEgypt.Web.Controllers;
[Authorize]
public class PaymentsController(IPaymentRepository _paymentRepository) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionUrl = await _paymentRepository.CreateCheckoutSessionAsync(userId!);

        if (string.IsNullOrEmpty(sessionUrl))
            return RedirectToAction("Index", "Cart");

        return Redirect(sessionUrl);
    }
}
