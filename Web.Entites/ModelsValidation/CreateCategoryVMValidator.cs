using FluentValidation;
using Web.Entites.ViewModels.CategoryVMs;

namespace Web.Entites.ModelsValidation;
public class CreateCategoryVMValidator:AbstractValidator<CreateCategoryVM>
{
    public CreateCategoryVMValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(30).WithMessage("Category name cannot exceed 30 characters.");
        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image is required.")
            .Must(ImageSignatureValidator.IsValidImage).WithMessage("Invalid image format. Only .jpg, .jpeg, .png are allowed.");
    }
}
