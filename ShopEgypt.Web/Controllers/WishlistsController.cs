using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Entites.ViewModels.WishlistVMs;

namespace ShopEgypt.Web.Controllers;
[Authorize]
public class WishlistsController(IWishlistRepository _wishlistRepository) : Controller
{

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(AddWishlistItem addWishlistItem, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isInWishlist = await _wishlistRepository.ToggelWishlistItemAsync(userId!, addWishlistItem, cancellationToken);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true, isInWishlist });
        else
            return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public async Task<IActionResult> Count(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var count = await _wishlistRepository.GetWishlistItemCountAsync(userId!, cancellationToken);
        return Json(new { count });
    }
}
