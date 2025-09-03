using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Moq;
using Web.Entites.ViewModels.ProductVMs;
using Xunit;

namespace Web.Entites.ModelsValidation.ProductValidations.Tests;

public class EditProductVMValidatorTests
{
    private IFormFile CreateImageWithSignature(
        string fileName = "image.png",
        string contentType = "image/png",
        byte[]? signatureBytes = null)
    {
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG
        var jpgSignature = new byte[] { 0xFF, 0xD8, 0xFF }; // JPG/JPEG

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
    public void EditProductVMValidator_WhenValidInput_ShouldPass()
    {
        var validator = new EditProductVMValidator();
        var model = new EditProductVM
        {
            Id = 1,
            Name = "Valid Product",
            Description = "This is a valid product description.",
            Price = 10.5m,
            CategoryId = 2,
            TotalStock = 5,
            ImageFile = CreateImageWithSignature()
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void EditProductVMValidator_WhenInvalidId_ShouldFail(int id)
    {
        var validator = new EditProductVMValidator();
        var model = new EditProductVM
        {
            Id = id,
            Name = "Valid",
            Price = 10,
            CategoryId = 1
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Id");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void EditProductVMValidator_WhenNameIsMissing_ShouldFail(string? name)
    {
        var validator = new EditProductVMValidator();
        var model = new EditProductVM
        {
            Id = 1,
            Name = name!,
            Price = 10,
            CategoryId = 1
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Name")
            .And.Contain(x => x.ErrorMessage == "Product name is required.");
    }

    [Fact]
    public void EditProductVMValidator_WhenNameTooLong_ShouldFail()
    {
        var validator = new EditProductVMValidator();
        var model = new EditProductVM
        {
            Id = 1,
            Name = new string('A', 51),
            Price = 10,
            CategoryId = 1
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Name")
            .And.Contain(x => x.ErrorMessage == "Product name must be at most 50 characters.");
    }

    [Fact]
    public void EditProductVMValidator_WhenDescriptionTooLong_ShouldFail()
    {
        var validator = new EditProductVMValidator();
        var model = new EditProductVM
        {
            Id = 1,
            Name = "Valid",
            Description = new string('D', 501),
            Price = 10,
            CategoryId = 1
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Description")
            .And.Contain(x => x.ErrorMessage == "Description must be at most 500 characters.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void EditProductVMValidator_WhenInvalidPrice_ShouldFail(decimal price)
    {
        var validator = new EditProductVMValidator();
        var model = new EditProductVM
        {
            Id = 1,
            Name = "Valid",
            Price = price,
            CategoryId = 1
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Price");
    }

    [Fact]
    public void EditProductVMValidator_WhenCategoryIdMissing_ShouldFail()
    {
        var validator = new EditProductVMValidator();
        var model = new EditProductVM
        {
            Id = 1,
            Name = "Valid",
            Price = 10,
            CategoryId = null
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "CategoryId")
            .And.Contain(x => x.ErrorMessage == "Category is required.");
    }

    [Fact]
    public void EditProductVMValidator_WhenInvalidImage_ShouldFail()
    {
        var validator = new EditProductVMValidator();
        var fakeFile = new Mock<IFormFile>().Object;

        var model = new EditProductVM
        {
            Id = 1,
            Name = "Valid",
            Price = 10,
            CategoryId = 1,
            ImageFile = fakeFile
        };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "ImageFile")
            .And.Contain(x => x.ErrorMessage == "Image must be a valid JPEG or PNG file.");
    }
}
