namespace Web.Entites.Models;
public class ApplicationUser:IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName {  get; set; }= string.Empty;
    public ICollection<ProductRating> ProductRatings { get; set; } = [];
}
