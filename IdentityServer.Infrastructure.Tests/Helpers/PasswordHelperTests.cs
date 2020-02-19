using System;
using FluentAssertions;
using IdentityServer.Infrastructure.Helpers;
using NUnit.Framework;

namespace IdentityServer.Infrastructure.Tests.Helpers
{
    [TestFixture]
    public class PasswordHelperTests
    {
        [Test]
        public void PasswordHelperGenerateSalt()
        {
            PasswordHelper.GenerateSalt()
                .Should().NotBeNull();
        }

        [Test]
        public void PasswordHelperHashPasswordSaltIsNullThrowArgumentNullException()
        {
            Action action = () => PasswordHelper.HashPassword(null, "a");
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void PasswordHelperHashPasswordPasswordIsNullThrowArgumentNullException()
        {
            Action action = () => PasswordHelper.HashPassword("a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void PasswordHelperHashPassword()
        {
            PasswordHelper.HashPassword("a", "a")
                .Should().NotBeNullOrEmpty();
        }

        [Test]
        public void PasswordHelperValidatePasswordSaltIsNullThrowArgumentNullException()
        {
            Action action = () => PasswordHelper.ValidateHashPassword(null, null, null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void PasswordHelperValidatePasswordPasswordIsNullThrowArgumentNullException()
        {
            Action action = () => PasswordHelper.ValidateHashPassword("a", null, null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void PasswordHelperValidatePasswordHashedPasswordIsNullThrowArgumentNullException()
        {
            Action action = () => PasswordHelper.ValidateHashPassword("a", "a", null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void PasswordHelperValidatePasswordInValid()
        {
            var hashedPassword = PasswordHelper.HashPassword("a", "a");
            PasswordHelper.ValidateHashPassword("a", "b", hashedPassword).Should().BeFalse();
        }

        [Test]
        public void PasswordHelperValidatePassword()
        {
            var hashedPassword = PasswordHelper.HashPassword("a", "a");
            PasswordHelper.ValidateHashPassword("a", "a", hashedPassword).Should().BeTrue();
        }
    }
}