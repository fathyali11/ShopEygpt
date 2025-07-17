namespace Web.Entites.ViewModels.UsersVMs;
public record RegisterVM(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
    );