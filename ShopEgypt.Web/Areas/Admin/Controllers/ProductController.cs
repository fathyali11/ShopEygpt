using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Web.Entites.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShopEgypt.Web.Areas.Admin.Controllers
{
    [Area(SD.AdminRole)]
    [Authorize(Roles = SD.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetData()
        {
            var products=_unitOfWork.ProductRepository.GetAll(includeObj:"Category").ToList();
            return Json(new {data=products});
        }
        [HttpGet]
        public IActionResult Create()
        {
            var productVM = new ProductVMCreate
            {
                Product = new Product(),
                CategoryList = _unitOfWork.CategoryRepository.CategorySelectList()
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVMCreate model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.AddProductVM(model);
                _unitOfWork.Save();
                TempData["Success"] = "Data Created Successfly";
                return RedirectToAction("Index");
            }
            model.CategoryList = _unitOfWork.CategoryRepository.CategorySelectList();
            TempData["Error"] = "Data Not Created";
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var productVM = new ProductVMEdit
            {
                Product = _unitOfWork.ProductRepository.GetBy(x => x.Id == id, includeObj: "Category"),
                CategoryList=_unitOfWork.CategoryRepository.CategorySelectList()
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVMEdit model)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(model);
                _unitOfWork.Save(); 
                TempData["Success"] = "Data Updated Successfly";
                return RedirectToAction("Index");
            }
            model.CategoryList = _unitOfWork.CategoryRepository.CategorySelectList();
            TempData["Error"] = "Data Not Updated";
            return View(model);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.ProductRepository.GetBy(x => x.Id == id);

            if (product != null)
            {
                _unitOfWork.ProductRepository.DeleteWithImage(product);
                var res = _unitOfWork.Save();

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
            else
            {
                TempData["Error"] = "Data not found";
                return Json(new { success = false, message = "Data not found" });
            }
        }

    }
}
