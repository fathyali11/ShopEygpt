

namespace ShopEgypt.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(category);
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
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(category);
                TempData["Success"] = "Data Updated Successfly";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Data Not Updated";
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.CategoryRepository.GetBy(x => x.Id == id);
            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Save();
            TempData["Success"] = "Data Remove Successfly";
            return RedirectToAction("Index");
        }
    }
}
