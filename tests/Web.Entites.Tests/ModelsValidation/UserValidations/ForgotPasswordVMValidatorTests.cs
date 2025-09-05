namespace Web.Tests.ModelsValidation.UserValidations;
public class ForgotPasswordVMValidatorTests
{
    [Fact]
    public void ForgotPasswordVMValidator_WhenValidEmail_ShouldPass()
    {
        // Arrange
        var validator = new ForgotPasswordVMValidator();
        var model = new ForgotPasswordVM("user@example.com");

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    [Fact]
    public void ForgotPasswordVMValidator_WhenEmailIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var validator = new ForgotPasswordVMValidator();
        var model = new ForgotPasswordVM("");

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" &&
                                            e.ErrorMessage == "Email id not empty");
    }

    [Fact]
    public void ForgotPasswordVMValidator_WhenEmailIsInvalid_ShouldReturnValidationError()
    {
        // Arrange
        var validator = new ForgotPasswordVMValidator();
        var model = new ForgotPasswordVM("invalid-email");

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" &&
                                            e.ErrorMessage == "enter correct email");
    }
}
