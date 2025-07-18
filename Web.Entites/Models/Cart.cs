namespace Web.Entites.Models;
public class Cart
{
    public int Id {  get; set; }
    public string UserId {  get; set; }= string.Empty;

    public List<CartItem> CartItems { get; set; } = [];
}
