using FluentValidation;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.Entites.ModelsValidation.UserValidations;

public class ResendEmailConfirmationVMValidator : AbstractValidator<ResendEmailConfirmationVM>
{
    public ResendEmailConfirmationVMValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email id not empty");
    }
}
