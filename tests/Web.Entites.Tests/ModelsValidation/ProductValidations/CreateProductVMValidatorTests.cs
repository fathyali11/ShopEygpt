namespace Web.Entites.ModelsValidation.ProductValidations.Tests;
public class CreateProductVMValidatorTests
{
    private IFormFile CreateImageWithSignature(
        string fileName = "image.png",
        string contentType = "image/png",
        byte[]? signatureBytes = null)
    {
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG
        var jpgSignature = new byte[] { 0xFF, 0xD8, 0xFF };        // JPG/JPEG

        signatureBytes ??= fileName.EndsWith(".png") ? pngSignature : jpgSignature;

        var bytes = signatureBytes.Concat(new byte[1024]).ToArray();
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public void CreateProductVMValidator_WhenValidInput_ShouldPass()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            "Valid Product",
            "Valid description",
            100.50m,
            1,
            10,
            CreateImageWithSignature()
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    [Fact]
    public void CreateProductVMValidator_WhenNameIsEmpty_ShouldReturnValidationError()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            string.Empty,
            "desc",
            50,
            1,
            5,
            CreateImageWithSignature()
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" &&
                                            e.ErrorMessage == "Product name is required.");
    }

    [Fact]
    public void CreateProductVMValidator_WhenNameTooLong_ShouldReturnValidationError()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            new string('a', 51),
            "desc",
            50,
            1,
            5,
            CreateImageWithSignature()
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" &&
                                            e.ErrorMessage == "Product name must be at most 50 characters.");
    }

    [Fact]
    public void CreateProductVMValidator_WhenDescriptionTooLong_ShouldReturnValidationError()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            "name",
            new string('d', 501),
            50,
            1,
            5,
            CreateImageWithSignature()
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" &&
                                            e.ErrorMessage == "Description must be at most 500 characters.");
    }

    [Fact]
    public void CreateProductVMValidator_WhenPriceIsZeroOrNegative_ShouldReturnValidationError()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            "name",
            "desc",
            0,
            1,
            5,
            CreateImageWithSignature()
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price" &&
                                            e.ErrorMessage == "Price must be greater than 0.");
    }

    [Fact]
    public void CreateProductVMValidator_WhenCategoryIdIsEmpty_ShouldReturnValidationError()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            "name",
            "desc",
            100,
            0,
            5,
            CreateImageWithSignature()
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId" &&
                                            e.ErrorMessage == "Category is required.");
    }

    [Fact]
    public void CreateProductVMValidator_WhenTotalStockIsNegative_ShouldReturnValidationError()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            "name",
            "desc",
            100,
            1,
            -5,
            CreateImageWithSignature()
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TotalStock" &&
                                            e.ErrorMessage == "Stock quantity must be a non-negative integer.");
    }

    [Fact]
    public void CreateProductVMValidator_WhenImageInvalid_ShouldReturnValidationError()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            "name",
            "desc",
            100,
            1,
            5,
            new Mock<IFormFile>().Object
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ImageFile" &&
                                            e.ErrorMessage == "Image must be a valid JPEG or PNG file.");
    }

    [Fact]
    public void CreateProductVMValidator_WhenImageValid_ShouldPass()
    {
        var validator = new CreateProductVMValidator();
        var model = new CreateProductVM(
            "name",
            "desc",
            100,
            1,
            5,
            CreateImageWithSignature("valid.png")
        );

        var result = validator.Validate(model);

        result.IsValid.Should().BeTrue();
    }
}
