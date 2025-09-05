namespace Web.Entites.ModelsValidation.UserValidations;
public class ConfirmEmailVMValidator:AbstractValidator<ConfirmEmailVM>
{
    public ConfirmEmailVMValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("user id not empty");
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token not empty");
    }
}
