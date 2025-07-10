using FluentValidation;
using Web.Entites.ViewModels.ProductVMs;

namespace Web.Entites.ModelsValidation.ProductValidations;
public class CreateProductVMValidator:AbstractValidator<CreateProductVM>
{
    public CreateProductVMValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(50).WithMessage("Product name must be at most 50 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters.");
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
        RuleFor(x => x.ImageFile)
            .Must(ImageSignatureValidator.IsValidImage!)
            .WithMessage("Image must be a valid JPEG or PNG file.");
    }
}