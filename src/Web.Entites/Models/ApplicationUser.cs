namespace Web.Entites.Models;
public class ApplicationUser:IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName {  get; set; }= string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<ProductRating> ProductRatings { get; set; } = [];
    public ICollection<UserRecommendation> UserRecommendations { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
