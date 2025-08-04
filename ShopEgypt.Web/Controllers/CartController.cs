using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Entites.ViewModels.CartItemVMs;

namespace ShopEgypt.Web.Controllers;

[Authorize]
public class CartController(ICartRepository _cartRepository) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartRepository.GetCartItemsAsync(userId!, cancellationToken);
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddCartItemVM addCartItemVM,CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _cartRepository.AddToCartAsync(userId!, addCartItemVM, cancellationToken);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true, message = "The product was added successfully!" });
        else
            return RedirectToAction("Index", "Home");

    }
    [HttpGet]
    public async Task<IActionResult> Count(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var count = await _cartRepository.GetCartItemCountAsync(userId!, cancellationToken);
        return Json(new { count });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Increase(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var (count, totalPrice) = await _cartRepository.IncreaseAsync(cartItemVM, cancellationToken);
        return Json(new { count, totalPrice });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decrease(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var (count, totalPrice) = await _cartRepository.DecreaseAsync(cartItemVM, cancellationToken);
        return Json(new { count, totalPrice });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Delete_Increase_DecreaseCartItemVM cartItemVM, CancellationToken cancellationToken = default)
    {
        var totalPrice=await _cartRepository.DeleteCartItemAsync(cartItemVM, cancellationToken);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true, message = "The product was deleted successfully!",totalPrice });
        else
            return RedirectToAction("Index", "Home");

    }



}
