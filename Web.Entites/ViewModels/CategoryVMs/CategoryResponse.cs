namespace Web.Entites.ViewModels.CategoryVMs;
public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }=string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
