using System.Linq;
using FluentAssertions;
using FluentValidation;
using IdentityServer.Infrastructure.RequestModels;
using IdentityServer.Infrastructure.Validators;
using NUnit.Framework;

namespace IdentityServer.Infrastructure.Tests.Validators
{
    [TestFixture]
    public class SignUpRequestValidationTests
    {
        private IValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new SignUpRequestValidator();
        }

        [Test]
        public void ValidateRequestAllParametersAreNull()
        {
            var request = new SignUpRequest
            {
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();

            var errors = result.Errors.Any(x => x.ErrorCode == ValidationErrorCodes.NotNullValidator.ToString());
            errors.Should().BeTrue();
        }

        [Test]
        public void ValidateRequestAllParametersAreEmpty()
        {
            var request = new SignUpRequest
            {
                Username = "",
                UserMail = "",
                Password = "",
                ConfirmPassword = ""
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();

            var errors = result.Errors.Any(x => x.ErrorCode == ValidationErrorCodes.NotEmptyValidator.ToString());
            errors.Should().BeTrue();
        }

        [Test]
        public void ValidateRequestUserMailIsInvalid()
        {
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a",
                Password = "123",
                ConfirmPassword = "123"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();

            var errors = result.Errors.Any(x => x.ErrorCode == ValidationErrorCodes.EmailValidator.ToString());
            errors.Should().BeTrue();
        }
        
        [Test]
        public void ValidateRequestPasswordAndConfirmPasswordIsNotEqual()
        {
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a@a.com",
                Password = "123",
                ConfirmPassword = "124"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();

            var errors = result.Errors.Any(x => x.ErrorCode == ValidationErrorCodes.EqualValidator.ToString());
            errors.Should().BeTrue();
        }
        
        [Test]
        public void ValidateRequestAllParametersAreValid()
        {
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a@a.com",
                Password = "123",
                ConfirmPassword = "123"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeTrue();
        }
    }
}