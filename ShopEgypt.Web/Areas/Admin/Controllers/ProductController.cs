using Microsoft.AspNetCore.Mvc;
using Web.Entites.ViewModels;

namespace ShopEgypt.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products=_unitOfWork.ProductRepository.GetAll(includeObj:"Category");
            return View(products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var productVM = new ProductVM
            {
                Product = new Product(),
                CategoryList = _unitOfWork.CategoryRepository.CategorySelectList()
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.AddProductVM(model);
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            model.CategoryList = _unitOfWork.CategoryRepository.CategorySelectList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var productVM = new ProductVM
            {
                Product = _unitOfWork.ProductRepository.GetBy(x => x.Id == id, includeObj: "Category"),
                CategoryList=_unitOfWork.CategoryRepository.CategorySelectList()
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM model)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(model);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            model.CategoryList = _unitOfWork.CategoryRepository.CategorySelectList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var prodcut=_unitOfWork.ProductRepository.GetBy(x=>x.Id == id);
            _unitOfWork.ProductRepository.DeleteWithImage(prodcut);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
