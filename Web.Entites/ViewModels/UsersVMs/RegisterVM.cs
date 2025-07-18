using System.ComponentModel;

namespace Web.Entites.ViewModels.UsersVMs;
public record RegisterVM(
    [Required]
    string FirstName,
    [Required]
    string LastName,
    [Required]
    string UserName,
    [EmailAddress]
    string Email,
    [PasswordPropertyText]
    string Password,
    [PasswordPropertyText]
    string ConfirmPassword
    );