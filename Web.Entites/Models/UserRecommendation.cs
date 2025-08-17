namespace Web.Entites.Models;
public class UserRecommendation
{
    public string UserId { get; set; }=string.Empty;
    public int ProductId { get; set; }
    public float Score { get; set; }
}