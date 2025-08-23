namespace WearUp.Web.Controllers;
public class OrdersController(IOrderRepository _orderRepository) : Controller
{
    [Authorize(Roles =UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Index(FilterRequest request, CancellationToken cancellationToken)
    {
        var response=await _orderRepository.GetAllOrdersAsync(request,cancellationToken);
        ViewData["SearchTerm"] = request.SearchTerm;
        ViewData["SortField"] = request.SortField;
        ViewData["SortOrder"] = request.SortOrder;
        return View(response);
    }
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var response = await _orderRepository.GetOrderDetailsAsync(id, cancellationToken);
        return View(response);
    }
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _orderRepository.CancelOrderAsync(userId!, id, cancellationToken);
        return response ?
            Json(new { Success = true, message = "Order Cancelled Successfull" }) :
            Json(new { Success = false, message = "Order Not Cancelled" });

    }
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _orderRepository.DeleteOrderAsync(userId!,id, cancellationToken);
        return response ?
            Json(new { Success = true, message = "Order Deleted Successfull" }) :
            Json(new { Success = false, message = "Order Not Deleted" });

    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Success(CancellationToken cancellationToken)
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionId = Request.Query["session_id"];

        var sessionService = new SessionService();
        var session = await sessionService.GetAsync(sessionId);

        if(string.Equals(session.PaymentStatus , PaymentStatus.Paid,StringComparison.OrdinalIgnoreCase))
        {
            var order = await _orderRepository.CreateOrderAsync(userId!, session.PaymentIntentId, session.Id,cancellationToken);
            return View(order);
        }

        return View("Failed");
    }
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Customer}")]
    [HttpGet]
    public IActionResult Failed()
    {
        return View(); 
    }
}
