namespace Web.Entites.ViewModels.UsersVMs;

public record ResetPasswordVM(
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    string NewPassword,

    [Required]
    string UserId,

    [Required]
    string Token
);
