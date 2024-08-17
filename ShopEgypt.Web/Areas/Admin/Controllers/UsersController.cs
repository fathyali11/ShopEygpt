using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShopEgypt.Web.Areas.Admin.Controllers
{
    [Area(SD.AdminRole)]
    [Authorize(Roles = SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = _unitOfWork.ApplicaionUserRepository.GetAll(x => x.Id != userId);
            return View(users);
        }
        public IActionResult GetData()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = _unitOfWork.ApplicaionUserRepository.GetAll(x => x.Id != userId);
            return Json(new {data=users});
        }
        [HttpGet]
        public IActionResult LockOrOpen(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = _unitOfWork.ApplicaionUserRepository.GetBy(x => x.Id == userId);
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

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index), "Users", new {area=SD.AdminRole});
        }
        public IActionResult Details(string userId)
        {
            var user=_unitOfWork.ApplicaionUserRepository.GetBy(x=>x.Id == userId);
            if(user == null)
                return NotFound();
            return View(user);
        }
        public IActionResult Edit(ApplicationUser user)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.ApplicaionUserRepository.Update(user);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

    }
}
