using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShopEgypt.Web.Areas.Admin.Controllers
{
    [Area(SD.AdminRole)]
    [Authorize(Roles = SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = _context.ApplicationUsers.Where(x => x.Id != userId);
            return View(users);
        }
        public IActionResult GetData()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = _context.ApplicationUsers.Where(x => x.Id != userId);
            return Json(new {data=users});
        }
        [HttpGet]
        public IActionResult LockOrOpen(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return NotFound();

            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now + TimeSpan.FromDays(4);
            }
            else
            {
                user.LockoutEnd = null;
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index), "Users", new {area=SD.AdminRole});
        }

    }
}
