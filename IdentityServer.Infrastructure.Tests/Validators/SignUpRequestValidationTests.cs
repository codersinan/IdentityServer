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
        public void ValidateAllParametersAreNull()
        {
            var request = new SignUpRequest()
            {
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().Be(false);

            var errors = result.Errors.Any(x => x.ErrorCode == ValidationErrorCodes.NotNullValidator.ToString());
            errors.Should().Be(true);
        }

        [Test]
        public void ValidateAllParametersAreEmpty()
        {
            var request = new SignUpRequest
            {
                Username = "",
                UserMail = "",
                Password = "",
                ConfirmPassword = ""
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().Be(false);

            var errors = result.Errors.Any(x => x.ErrorCode == ValidationErrorCodes.NotEmptyValidator.ToString());
            errors.Should().Be(true);
        }
    }
}