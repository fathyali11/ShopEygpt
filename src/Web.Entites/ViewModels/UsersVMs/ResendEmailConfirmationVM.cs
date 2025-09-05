namespace Web.Entites.ViewModels.UsersVMs;

using System.ComponentModel.DataAnnotations;
public record ResendEmailConfirmationVM(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a correct email")]
    string Email
);
