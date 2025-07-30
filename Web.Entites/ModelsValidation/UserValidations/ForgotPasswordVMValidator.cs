using FluentValidation;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.Entites.ModelsValidation.UserValidations;

public class ForgotPasswordVMValidator : AbstractValidator<ForgotPasswordVM>
{
    public ForgotPasswordVMValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email id not empty")
            .EmailAddress().WithMessage("enter correct email");
    }
}

