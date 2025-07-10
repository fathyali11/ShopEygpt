using System.Threading.Tasks;
using Web.Entites.ViewModels.ProductVMs;

namespace ShopEgypt.Web.Controllers;

public class ProductController(IProductRepository _productRepositoy) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var response=await _productRepositoy.GetAllProductsAdminAsync();
        return View(response);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateProductVM(default!,default!, default!, default!, default!));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductVM model,CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result=await _productRepositoy.AddProductAsync(model, cancellationToken);
        if(result.IsT1)
        {
            TempData["Success"] = "Data Created Successfully";
            return RedirectToAction("Index");
        }
        else
        {
            result.AsT0.ForEach(error => ModelState.AddModelError(error.PropertyName, error.ErrorMessage));
            TempData["Error"] = "Data Not Created";
            return View(model);
        }
    }
    //[HttpGet]
    //public IActionResult Edit(int id)
    //{
    //    var productVM = new ProductVMEdit
    //    {
    //        Product = _unitOfWork.ProductRepository.GetBy(x => x.Id == id, includeObj: "Category"),
    //        CategoryList = _unitOfWork.CategoryRepository.CategorySelectList()
    //    };
    //    return View(productVM);
    //}
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Edit(ProductVMEdit model)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        _unitOfWork.ProductRepository.Update(model);
    //        _unitOfWork.Save();
    //        TempData["Success"] = "Data Updated Successfly";
    //        return RedirectToAction("Index");
    //    }
    //    model.CategoryList = _unitOfWork.CategoryRepository.CategorySelectList();
    //    TempData["Error"] = "Data Not Updated";
    //    return View(model);
    //}
    //[HttpDelete]
    //public IActionResult Delete(int id)
    //{
    //    var product = _unitOfWork.ProductRepository.GetBy(x => x.Id == id);

    //    if (product != null)
    //    {
    //        _unitOfWork.ProductRepository.DeleteWithImage(product);
    //        var res = _unitOfWork.Save();

    //        if (res != 0)
    //        {
    //            TempData["Success"] = "Data removed successfully";
    //            return Json(new { success = true, message = "Data deleted successfully" });
    //        }
    //        else
    //        {
    //            TempData["Error"] = "Data not removed";
    //            return Json(new { success = false, message = "Data deletion failed" });
    //        }
    //    }
    //    else
    //    {
    //        TempData["Error"] = "Data not found";
    //        return Json(new { success = false, message = "Data not found" });
    //    }
    //}

}
