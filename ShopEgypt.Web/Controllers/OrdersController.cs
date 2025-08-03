using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace ShopEgypt.Web.Controllers;

public class OrdersController(IOrderRepository _orderRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Success()
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionId = Request.Query["session_id"];

        var sessionService = new SessionService();
        var session = await sessionService.GetAsync(sessionId);

        if(session.PaymentStatus==PaymentStatus.PaymentStatusPaid)
        {
            var order = await _orderRepository.CreateOrderAsync(userId, session.PaymentIntentId, session.Id);
            return View(order);
        }

        return View();
    }

}
