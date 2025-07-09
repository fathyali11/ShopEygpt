using FluentValidation;

namespace Web.Entites.ModelsValidation.CategoryValidations;

public class EditCategoryVMValidator : AbstractValidator<EditCategoryVM>
{
    public EditCategoryVMValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MinimumLength(3).WithMessage("Category name must be at least 3 characters long.")
            .MaximumLength(30).WithMessage("Category name cannot exceed 30 characters.");
        

        RuleFor(x => x.Image)
            .NotNull()
            .When(x => x.Image != null)
            .Must(ImageSignatureValidator.IsValidImage!).When(x => x.Image != null)
            .WithMessage("Invalid image format. Only .jpg, .jpeg, .png are allowed.");
    }
}
