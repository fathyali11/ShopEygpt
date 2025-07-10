namespace ShopEgypt.Web.ViewComponents;

public class CategorySelectListViewComponent(ICategoryRepository _categoryRepository): ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryRepository.GetAllCategoriesSelectListAsync();
        return View(categories);
    }
}
