using FluentAssertions;
using Web.Entites.ModelsValidation.UserValidations;
using Web.Entites.ViewModels.UsersVMs;
using Xunit;

namespace Web.Tests.ModelsValidation.UserValidations
{
    public class ResendEmailConfirmationVMValidatorTests
    {
        [Fact]
        public void ResendEmailConfirmationVMValidator_WhenValidEmail_ShouldPass()
        {
            // Arrange
            var validator = new ResendEmailConfirmationVMValidator();
            var model = new ResendEmailConfirmationVM("user@example.com");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public void ResendEmailConfirmationVMValidator_WhenEmailIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var validator = new ResendEmailConfirmationVMValidator();
            var model = new ResendEmailConfirmationVM("");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email" &&
                                                e.ErrorMessage == "Email id not empty");
        }

        [Fact]
        public void ResendEmailConfirmationVMValidator_WhenEmailIsInvalid_ShouldReturnValidationError()
        {
            // Arrange
            var validator = new ResendEmailConfirmationVMValidator();
            var model = new ResendEmailConfirmationVM("invalid-email");

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email" &&
                                                e.ErrorMessage == "enter correct email");
        }
    }
}
