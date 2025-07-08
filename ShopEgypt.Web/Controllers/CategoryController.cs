using Web.Entites.ViewModels.CategoryVMs;

namespace ShopEgypt.Web.Controllers;

public class CategoryController(ICategoryRepository _categoryRepository) : Controller
{
    //public IActionResult Index()
    //{
    //    var categories = _unitOfWork.CategoryRepository.GetAll().ToList();
    //    return View(categories);
    //}
    //public IActionResult GetData()
    //{
    //    var categories=_unitOfWork.CategoryRepository.GetAll().ToList();
    //    return Json(new {data=categories});
    //}
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateCategoryVM(null!,null!));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryVM model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _categoryRepository.AddCategoryAsync(model, cancellationToken);

        if (result.IsT1)
        {
            TempData["Success"] = "Data Created Successfully";
            return RedirectToAction("Index");
        }

        var errors = result.AsT0;
        foreach (var error in errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

        TempData["Error"] = "Data Not Created";
        return View(model);
    }

    //[HttpGet]
    //public IActionResult Edit(int id)
    //{
    //    var category = _unitOfWork.CategoryRepository.GetBy(x => x.Id == id);
    //    return View(category);
    //}
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Edit(Category category,IFormFile ?imageCover)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        if(imageCover is not null) 
    //            category.ImageCover=imageCover;
    //        _unitOfWork.CategoryRepository.Update(category);
    //        _unitOfWork.Save();
    //        TempData["Success"] = "Data Updated Successfly";
    //        return RedirectToAction("Index");
    //    }
    //    TempData["Error"] = "Data Not Updated";
    //    return View(category);
    //}
    //[HttpDelete]
    //public IActionResult Delete(int id)
    //{
    //    if (id == 0)
    //        return NotFound();
    //    var category = _unitOfWork.CategoryRepository.GetBy(x => x.Id == id);
    //    _unitOfWork.CategoryRepository.DeleteWithImage(category);
    //    var res= _unitOfWork.Save();
    //    if (res != 0)
    //    {
    //        TempData["Success"] = "Data removed successfully";
    //        return Json(new { success = true, message = "Data deleted successfully" });
    //    }
    //    else
    //    {
    //        TempData["Error"] = "Data not removed";
    //        return Json(new { success = false, message = "Data deletion failed" });
    //    }
    //}
}
