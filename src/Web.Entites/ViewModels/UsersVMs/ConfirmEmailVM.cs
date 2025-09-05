namespace Web.Entites.ViewModels.UsersVMs;
public record ConfirmEmailVM(
    [Required(ErrorMessage ="UserId is required")]
    string UserId,
    [Required(ErrorMessage = "Token is required")]
     string Token);