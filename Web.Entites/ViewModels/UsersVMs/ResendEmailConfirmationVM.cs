namespace Web.Entites.ViewModels.UsersVMs;

public record ResendEmailConfirmationVM(
    [Required(ErrorMessage ="Email is required")]
    string Email);