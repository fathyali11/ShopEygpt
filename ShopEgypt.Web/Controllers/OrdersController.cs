using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;
using System.Security.Claims;

namespace ShopEgypt.Web.Controllers;

[Authorize]
public class OrdersController(IOrderRepository _orderRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Success(CancellationToken cancellationToken)
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionId = Request.Query["session_id"];

        var sessionService = new SessionService();
        var session = await sessionService.GetAsync(sessionId);

        if(session.PaymentStatus==PaymentStatus.Paid)
        {
            var order = await _orderRepository.CreateOrderAsync(userId!, session.PaymentIntentId, session.Id,cancellationToken);
            return View(order);
        }

        return View("Failed");
    }
    [HttpGet]
    public IActionResult Failed()
    {
        return View(); 
    }
}
