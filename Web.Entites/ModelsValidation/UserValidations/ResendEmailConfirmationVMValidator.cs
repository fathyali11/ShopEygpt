namespace Web.Entites.ModelsValidation.UserValidations;
public class ResendEmailConfirmationVMValidator : AbstractValidator<ResendEmailConfirmationVM>
{
    public ResendEmailConfirmationVMValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email id not empty")
            .EmailAddress().WithMessage("enter correct email");
    }
}
