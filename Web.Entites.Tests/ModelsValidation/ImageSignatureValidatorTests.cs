using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Text;
using Web.Entites.ModelsValidation;
using Xunit;

public class ImageSignatureValidatorTests
{
    private IFormFile CreateFormFile(byte[] fileBytes, string fileName = "test.jpg")
    {
        var stream = new MemoryStream(fileBytes);
        return new FormFile(stream, 0, fileBytes.Length, "file", fileName);
    }

    [Fact]
    public void IsValidImage_FileIsNull_ReturnsFalse()
    {
        // Act
        var result = ImageSignatureValidator.IsValidImage(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidImage_FileTooSmall_ReturnsFalse()
    {
        // Arrange
        var file = CreateFormFile(new byte[] { 0xFF });

        // Act
        var result = ImageSignatureValidator.IsValidImage(file);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidImage_ValidJpeg_ReturnsTrue()
    {
        // Arrange (JPEG starts with FF D8 FF)
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10 };
        var file = CreateFormFile(jpegBytes, "image.jpg");

        // Act
        var result = ImageSignatureValidator.IsValidImage(file);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidImage_ValidPng_ReturnsTrue()
    {
        // Arrange (PNG starts with 89 50 4E 47 0D 0A 1A 0A)
        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var file = CreateFormFile(pngBytes, "image.png");

        // Act
        var result = ImageSignatureValidator.IsValidImage(file);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidImage_InvalidFile_ReturnsFalse()
    {
        // Arrange (Random bytes not matching JPEG/PNG signatures)
        var invalidBytes = Encoding.UTF8.GetBytes("This is not an image");
        var file = CreateFormFile(invalidBytes, "text.txt");

        // Act
        var result = ImageSignatureValidator.IsValidImage(file);

        // Assert
        result.Should().BeFalse();
    }
}
