namespace Web.Entites.ViewModels.UsersVMs;

public record ResendEmailConfirmationVM(
    [Required(ErrorMessage ="Email is required")]
    [EmailAddress(ErrorMessage ="enter correct email")]
    string Email);