//using Microsoft.EntityFrameworkCore.Metadata.Internal;

//namespace ShopEgypt.Web.ViewComponents
//{
//    public class UsersViewComponent:ViewComponent
//    {
//        private readonly IUnitOfWork _unitOfWork;

//        public UsersViewComponent(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            var numberOfUsers=_unitOfWork.ApplicaionUserRepository.GetAll().Count();
//            return View("UsersCount", numberOfUsers);
//        }

//    }
//}
