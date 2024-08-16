

namespace ShopEgypt.Web.Areas.Admin.Controllers
{
    [Area(SD.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = _unitOfWork.CategoryRepository.GetAll().ToList();
            return View(categories);
        }
        public IActionResult GetData()
        {
            var categories=_unitOfWork.CategoryRepository.GetAll().ToList();
            return Json(new {data=categories});
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category,IFormFile imageCover)
        {
            if (ModelState.IsValid)
            {
                category.ImageCover = imageCover;
                _unitOfWork.CategoryRepository.AddWithImage(category);
                _unitOfWork.Save();
                TempData["Success"] = "Data Created Successfly";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Data Not Created";
            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetBy(x => x.Id == id);
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category,IFormFile ?imageCover)
        {
            if (ModelState.IsValid)
            {
                if(imageCover is not null) 
                    category.ImageCover=imageCover;
                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.Save();
                TempData["Success"] = "Data Updated Successfly";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Data Not Updated";
            return View(category);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.CategoryRepository.GetBy(x => x.Id == id);
            _unitOfWork.CategoryRepository.DeleteWithImage(category);
            var res= _unitOfWork.Save();
            if (res != 0)
            {
                TempData["Success"] = "Data removed successfully";
                return Json(new { success = true, message = "Data deleted successfully" });
            }
            else
            {
                TempData["Error"] = "Data not removed";
                return Json(new { success = false, message = "Data deletion failed" });
            }
        }
    }
}
