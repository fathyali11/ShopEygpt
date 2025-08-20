namespace WearUp.Web.Controllers;
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
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await _wishlistRepository.GetWishlistItems(userId!, cancellationToken);
        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(DeleteWishlistItem deleteWishlistItem, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var wishlistItemsCount=await _wishlistRepository.DeleteWishlistItemAsync(userId,deleteWishlistItem, cancellationToken);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            if(wishlistItemsCount != -1)
                return Json(new { success = true, message = "The product was deleted successfully!", wishlistItemsCount });
            else
                return Json(new { success = false, message = "The product was not deleted." });
        else
            return RedirectToAction("Index", "Home");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(int wishlistId,CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _wishlistRepository.ClearWishlistAsync(wishlistId,userId!, cancellationToken);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true, message = "The wishlist was cleared successfully!" });
        else
            return RedirectToAction("Index", "Home");
    }

}
