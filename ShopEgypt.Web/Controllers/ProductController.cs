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
    //[HttpGet]
    //public async Task<IActionResult> GetAllInCategory(int categoryId,CancellationToken cancellationToken)
    //{
    //    var products=await _productRepositoy.GetAllProductsInCategoryAsync(categoryId,cancellationToken);

    //}

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
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product= await _productRepositoy.GetProductEditByIdAsync(id);
        if (product == null)
        {
            TempData["Error"] = "Product Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View(product);

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAsync(EditProductVM model,CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid)
            return View(model);

        var result = await _productRepositoy.UpdateProductAsync(model, cancellationToken);

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
        await _productRepositoy.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }

}
