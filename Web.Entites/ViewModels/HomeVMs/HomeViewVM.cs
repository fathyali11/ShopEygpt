using Web.Entites.ViewModels.CategoryVMs;
using Web.Entites.ViewModels.ProductVMs;

namespace Web.Entites.ViewModels.HomeVMs;
public class HomeViewVM
{
    public List<DiscoverProductVM> ProductsForDiscover { get; set; } = [];
    public List<CategoryResponse> CategoriesResponse { get; set; } = [];
    public List<NewArrivalProductsVM> NewArrivalProducts { get; set; } = [];
    public List<BestSellingProductVM> BestSellingProducts { get; set; } = [];

}
