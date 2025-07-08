using FluentValidation;

namespace Web.Entites.ModelsValidation;
public class CreateCategoryVMValidator:AbstractValidator<CreateCategoryVM>
{
    public CreateCategoryVMValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MinimumLength(3).WithMessage("Category name must be at least 3 characters long.")
            .MaximumLength(30).WithMessage("Category name cannot exceed 30 characters.");
        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image is required.")
            .Must(ImageSignatureValidator.IsValidImage).WithMessage("Invalid image format. Only .jpg, .jpeg, .png are allowed.");
    }
}
