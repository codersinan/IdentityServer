using System.Linq;
using FluentAssertions;
using FluentValidation;
using IdentityServer.Infrastructure.RequestModels;
using IdentityServer.Infrastructure.Validators;
using NUnit.Framework;

namespace IdentityServer.Infrastructure.Tests.Validators
{
    [TestFixture]
    public class SignInRequestValidationTests
    {
        private IValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new SignInRequestValidator();
        }

        [Test]
        public void ValidateRequestAllParametersAreNull()
        {
            var request = new SignInRequest
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
            var request = new SignInRequest
            {
                Username = "",
                Password = "",
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();

            var errors = result.Errors.Any(x => x.ErrorCode == ValidationErrorCodes.NotEmptyValidator.ToString());
            errors.Should().BeTrue();
        }

        [Test]
        public void ValidateRequestAllParametersAreValid()
        {
            var request = new SignInRequest
            {
                Username = "a",
                Password = "123",
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeTrue();
        }
    }
}