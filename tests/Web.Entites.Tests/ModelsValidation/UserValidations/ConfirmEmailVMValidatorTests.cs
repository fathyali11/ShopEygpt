using FluentAssertions;
using Web.Entites.ModelsValidation.UserValidations;
using Web.Entites.ViewModels.UsersVMs;
using Xunit;

namespace Web.Tests.ModelsValidation.UserValidations
{
    public class ConfirmEmailVMValidatorTests
    {
        [Fact]
        public void ConfirmEmailVMValidator_WhenValidInput_ShouldPass()
        {
            // Arrange
            var validator = new ConfirmEmailVMValidator();
            var model = new ConfirmEmailVM("validUserId", "validToken");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public void ConfirmEmailVMValidator_WhenUserIdIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var validator = new ConfirmEmailVMValidator();
            var model = new ConfirmEmailVM("", "validToken");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "UserId" &&
                                                e.ErrorMessage == "user id not empty");
        }

        [Fact]
        public void ConfirmEmailVMValidator_WhenTokenIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var validator = new ConfirmEmailVMValidator();
            var model = new ConfirmEmailVM("validUserId", "");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Token" &&
                                                e.ErrorMessage == "Token not empty");
        }

        [Fact]
        public void ConfirmEmailVMValidator_WhenBothFieldsAreEmpty_ShouldReturnValidationErrors()
        {
            // Arrange
            var validator = new ConfirmEmailVMValidator();
            var model = new ConfirmEmailVM("", "");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "UserId" &&
                                                e.ErrorMessage == "user id not empty");
            result.Errors.Should().Contain(e => e.PropertyName == "Token" &&
                                                e.ErrorMessage == "Token not empty");
        }
    }
}
