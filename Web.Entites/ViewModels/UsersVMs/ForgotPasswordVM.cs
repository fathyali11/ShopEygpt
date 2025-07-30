namespace Web.Entites.ViewModels.UsersVMs;

public record ForgotPasswordVM(
    [Required(ErrorMessage ="Email is required")]
    [EmailAddress(ErrorMessage ="enter correct email")]
    string Email);