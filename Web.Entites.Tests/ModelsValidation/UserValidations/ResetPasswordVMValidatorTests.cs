using FluentAssertions;
using Web.Entites.ModelsValidation.UserValidations;
using Web.Entites.ViewModels.UsersVMs;
using Xunit;

namespace Web.Tests.ModelsValidation.UserValidations
{
    public class ResetPasswordVMValidatorTests
    {
        [Fact]
        public void ResetPasswordVMValidator_WhenValidInput_ShouldPass()
        {
            // Arrange
            var validator = new ResetPasswordVMValidator();
            var model = new ResetPasswordVM("newStrongPassword", "validUserId", "validToken");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public void ResetPasswordVMValidator_WhenNewPasswordIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var validator = new ResetPasswordVMValidator();
            var model = new ResetPasswordVM("", "validUserId", "validToken");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == "NewPassword" &&
                e.ErrorMessage == "New password is required");
        }

       
        [Fact]
        public void ResetPasswordVMValidator_WhenUserIdIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var validator = new ResetPasswordVMValidator();
            var model = new ResetPasswordVM("newStrongPassword", "", "validToken");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == "UserId" &&
                e.ErrorMessage == "User ID is required");
        }

        [Fact]
        public void ResetPasswordVMValidator_WhenTokenIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var validator = new ResetPasswordVMValidator();
            var model = new ResetPasswordVM("newStrongPassword", "validUserId", "");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Token" &&
                e.ErrorMessage == "Token is required");
        }
    }
}
