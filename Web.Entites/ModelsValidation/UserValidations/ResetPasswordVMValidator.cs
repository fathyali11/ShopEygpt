using FluentValidation;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.Entites.ModelsValidation.UserValidations;

public class ResetPasswordVMValidator : AbstractValidator<ResetPasswordVM>
{
    public ResetPasswordVMValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");
    }
}