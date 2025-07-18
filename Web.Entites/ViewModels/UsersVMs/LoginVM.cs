using System.ComponentModel;

namespace Web.Entites.ViewModels.UsersVMs;

public record LoginVM(
    [Required]
    string UserName,
    [PasswordPropertyText]
    string Password
    );