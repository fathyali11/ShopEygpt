namespace Web.Entites.ViewModels.UsersVMs;
public record RegisterVM
{
    [Required]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    public string LastName { get; init; } = string.Empty;

    [Required]
    public string UserName { get; init; } = string.Empty;

    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    public string? Role { get; init; }

    [PasswordPropertyText]
    public string Password { get; init; } = string.Empty;

    [Compare(nameof(Password),ErrorMessage ="password and confirm password must be equal")]
    [PasswordPropertyText]
    public string ConfirmPassword { get; init; }=string.Empty;
}