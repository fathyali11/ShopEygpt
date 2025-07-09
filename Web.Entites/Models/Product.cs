namespace Web.Entites.Models;
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId {  get; set; }
    public Category Category { get; set; }=default!;
    public string ImageName { get; set; } = string.Empty;
}
