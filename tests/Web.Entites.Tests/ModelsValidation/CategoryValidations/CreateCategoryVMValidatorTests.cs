namespace Web.Entites.ModelsValidation.CategoryValidations.Tests;

public class CreateCategoryVMValidatorTests
{
    private IFormFile CreateImageWithSignature(
    string fileName = "image.png",
    string contentType = "image/png",
    byte[]? signatureBytes = null)
    {
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG
        var jpgSignature = new byte[] { 0xFF, 0xD8, 0xFF };        // JPG/JPEG

        signatureBytes ??= fileName.EndsWith(".png") ? pngSignature : jpgSignature;

        // combine signature + fake data
        var bytes = signatureBytes.Concat(new byte[1024]).ToArray();
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }


    [Fact()]
    public void CreateCategoryVMValidator_WhenValidInput_ShouldPath()
    {
        // arrange
        var validator = new CreateCategoryVMValidator();
        var model = new CreateCategoryVM("test", CreateImageWithSignature());

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);

    }

    [Theory]
    [InlineData("21")]
    [InlineData("2")]
    public void CreateCategoryVMValidator_WhenNameLengthLessThan3_ShouldReturnValidationError(string name)
    {
        // arrange
        var validator = new CreateCategoryVMValidator();
        var model=new CreateCategoryVM(name, CreateImageWithSignature());
        // act
        var result = validator.Validate(model);
        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("Name");
        result.Errors[0].ErrorMessage.Should().Be("Category name must be at least 3 characters long.");
    }
    [Fact]
    public void CreateCategoryVMValidator_WhenNameIsEmpty_ShouldReturnValidationError()
    {
        // arrange
        var validator = new CreateCategoryVMValidator();
        var model = new CreateCategoryVM(string.Empty, CreateImageWithSignature());
        // act
        var result = validator.Validate(model);
        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors[0].PropertyName.Should().Be("Name");
        result.Errors[0].ErrorMessage.Should().Be("Category name is required.");
        result.Errors[1].PropertyName.Should().Be("Name");
        result.Errors[1].ErrorMessage.Should().Be("Category name must be at least 3 characters long.");
    }
    [Theory]
    [InlineData("1111111111111111111111111111111")]
    public void CreateCategoryVMValidator_WhenNameLengthGreaterThan30_ShouldReturnValidationError(string name)
    {
        // arrange
        var validator = new CreateCategoryVMValidator();
        var model = new CreateCategoryVM(name, CreateImageWithSignature());
        // act
        var result = validator.Validate(model);
        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("Name");
        result.Errors[0].ErrorMessage.Should().Be("Category name cannot exceed 30 characters.");
    }



    [Fact]
    public void CreateCategoryVMValidator_WhenImageNotValid_ShouldReturnValidationError()
    {
        // arrange
        var validator = new CreateCategoryVMValidator();
        var model = new CreateCategoryVM("name", new Mock<IFormFile>().Object);

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors[0].PropertyName.Should().Be("Image");
        result.Errors[0].ErrorMessage.Should().Be("Invalid image format. Only .jpg, .jpeg, .png are allowed.");
    }
    [Fact]
    public void CreateCategoryVMValidator_WhenImageIsNull_ShouldReturnValidationError()
    {
        // arrange
        var validator = new CreateCategoryVMValidator();
        var model = new CreateCategoryVM("name", null!);

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors[0].PropertyName.Should().Be("Image");
        result.Errors[0].ErrorMessage.Should().Be("Image is required.");
    }
}