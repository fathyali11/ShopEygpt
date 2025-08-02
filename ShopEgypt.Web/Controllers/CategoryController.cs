using System.Threading.Tasks;

namespace ShopEgypt.Web.Controllers;

public class CategoryController(ICategoryRepository _categoryRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber, CancellationToken cancellationToken)
    {
        var response = await _categoryRepository.GetAllCategoriesAsync(pageNumber,cancellationToken);
        return View(response);
    }
    [HttpGet]
    public async Task<IActionResult> LoadLimitedCategoryInHome()
    {
        var response = await _categoryRepository.GetAllCategoriesInHomeAsync(false);
        return PartialView("_CategoriesHomePartial", response);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllCategoriesInHome(CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllCategoriesInHomeAsync(true, cancellationToken);
        return View(categories);
    }
    [HttpGet]
    public async Task<IActionResult> LoadCategorySelectedList(CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllCategoriesSelectListAsync(cancellationToken);
        return PartialView("_CategorySelectedListPartial",categories);
    }
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
            TempData["SuccessMessage"] = "Data Created Successfully";
            return RedirectToAction("Index");
        }

        var errors = result.AsT0;
        foreach (var error in errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

        TempData["ErrorMessage"] = "Data Not Created";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response =await _categoryRepository.GetCategoryAsync(id);
        if (response == null)
        {
            TempData["ErrorMessage"] = "Data Not Updated";
            return RedirectToAction(nameof(Index), "Category");
        }
        return View(response);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCategoryVM model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _categoryRepository.UpdateCategoryAsync(model,cancellationToken);

        if (result.IsT1)
        {
            TempData["SuccessMessage"] = "Data Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
            

        var errors = result.AsT0;
        foreach (var error in errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        TempData["ErrorMessage"] = "Data Not Updated";
        return View(model);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryRepository.DeleteCategoryAsync(id);

        return result.IsT1 ? Json(new { success = true, message = "Category deleted successfully." }) :
            Json(new { success = false, message = "Failed to delete category." });
    }
}
