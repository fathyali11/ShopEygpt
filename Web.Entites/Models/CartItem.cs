namespace Web.Entites.Models;

public class CartItem
{
    public int Id {  get; set; }
    public int ProductId {  get; set; }
    public int CartId {  get; set; }
    public int Count {  get; set; }
    public decimal TotalPrice => Product.Price * Count;

    public Cart Cart { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
