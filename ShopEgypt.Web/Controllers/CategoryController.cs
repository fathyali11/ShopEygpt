using System.Threading.Tasks;

namespace ShopEgypt.Web.Controllers;

public class CategoryController(ICategoryRepository _categoryRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var response = await _categoryRepository.GetAllCategoriesAsync();
        return View(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetCategoryInHome()
    {
        var response = await _categoryRepository.GetAllCategoriesAsync();
        return PartialView("_CategoriesHomePartial", response);
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
            TempData["Success"] = "Data Created Successfully";
            return RedirectToAction("Index");
        }

        var errors = result.AsT0;
        foreach (var error in errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

        TempData["Error"] = "Data Not Created";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category =await _categoryRepository.GetCategoryAsync(id);
        return View(category);
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
            TempData["Success"] = "Data Updated Successfly";
            return RedirectToAction(nameof(Index));
        }

        var errors = result.AsT0;
        foreach (var error in errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

        TempData["Error"] = "Data Not Updated";
        return View(model);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryRepository.DeleteCategoryAsync(id);
        if (result.IsT1)
        {
            TempData["Success"] = "Category deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to delete category.";
        }
        return RedirectToAction(nameof(Index));
    }
}
