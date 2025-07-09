//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace ShopEgypt.Web.ViewComponents
//{
//    public class ShoppingCartViewComponent:ViewComponent
//    {
//        private readonly IUnitOfWork _unitOfWork;

//        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            var claims=(ClaimsIdentity)User.Identity;
//            string? userId=claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if(userId is not null)
//            {
//                HttpContext.Session.SetInt32(SD.SessionKey, _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId).Count());
//                return View("CartItems", HttpContext.Session.GetInt32(SD.SessionKey));
//            }
//            else
//            {
//                HttpContext.Session.Clear();
//                return View("CartItems", 0);
//            }
//        }
//    }
//}
