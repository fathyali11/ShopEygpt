using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Moq;
using Xunit;

namespace Web.Entites.ModelsValidation.CategoryValidations.Tests;

public class EditCategoryVMValidatorTests
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

    [Fact]
    public void EditCategoryVMValidator_WhenValidInput_ShouldPass()
    {
        // arrange
        var validator = new EditCategoryVMValidator();
        var model = new EditCategoryVM(1, "valid name", "exist.png", CreateImageWithSignature());

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    public void EditCategoryVMValidator_WhenNameLengthLessThan3_ShouldReturnValidationError(string name)
    {
        // arrange
        var validator = new EditCategoryVMValidator();
        var model = new EditCategoryVM(1, name, "exist.png", null);

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" &&
                                            e.ErrorMessage == "Category name must be at least 3 characters long.");
    }

    [Fact]
    public void EditCategoryVMValidator_WhenNameIsEmpty_ShouldReturnValidationError()
    {
        // arrange
        var validator = new EditCategoryVMValidator();
        var model = new EditCategoryVM(1, string.Empty, "exist.png", null);

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" &&
                                            e.ErrorMessage == "Category name is required.");
    }

    [Theory]
    [InlineData("1111111111111111111111111111111")] // 31 chars
    public void EditCategoryVMValidator_WhenNameLengthGreaterThan30_ShouldReturnValidationError(string name)
    {
        // arrange
        var validator = new EditCategoryVMValidator();
        var model = new EditCategoryVM(1, name, "exist.png", null);

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" &&
                                            e.ErrorMessage == "Category name cannot exceed 30 characters.");
    }

    [Fact]
    public void EditCategoryVMValidator_WhenImageProvidedAndInvalid_ShouldReturnValidationError()
    {
        // arrange
        var validator = new EditCategoryVMValidator();
        var invalidImage = new Mock<IFormFile>().Object; // no valid signature
        var model = new EditCategoryVM(1, "name", "exist.png", invalidImage);

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Image" &&
                                            e.ErrorMessage == "Invalid image format. Only .jpg, .jpeg, .png are allowed.");
    }

    [Fact]
    public void EditCategoryVMValidator_WhenImageIsNullAndNameIsValid_ShouldPass()
    {
        // arrange
        var validator = new EditCategoryVMValidator();
        var model = new EditCategoryVM(1, "valid name", "exist.png", null);

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EditCategoryVMValidator_WhenImageProvidedAndValid_ShouldPass()
    {
        // arrange
        var validator = new EditCategoryVMValidator();
        var model = new EditCategoryVM(1, "valid name", "exist.png", CreateImageWithSignature("valid.png"));

        // act
        var result = validator.Validate(model);

        // assert
        result.IsValid.Should().BeTrue();
    }
}
