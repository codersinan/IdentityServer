using System;
using FluentAssertions;
using IdentityServer.Core.Entities;
using NUnit.Framework;

namespace IdentityServer.Core.Tests
{
    [TestFixture]
    public class AccountTests
    {
        [Test]
        public void AccountTest()
        {
            var account = new Account();

            account.Id = Guid.NewGuid();
            account.Id.Should().NotBeEmpty();

            account.Username = "a";
            account.Username.Should().NotBeNullOrEmpty();

            account.UserMail = "a@a.com";
            account.UserMail.Should().NotBeNullOrEmpty();

            account.PasswordSalt = "a";
            account.PasswordSalt.Should().NotBeEmpty();

            account.PasswordHash = "a";
            account.PasswordHash.Should().NotBeEmpty();

            account.CreatedAt = DateTime.UtcNow;
            account.CreatedAt.Should().BeBefore(DateTime.UtcNow);
            
            account.LastModifiedAt = null;
            account.LastModifiedAt.Should().BeNull();
            
            account.LastModifiedAt=DateTime.UtcNow;
            account.LastModifiedAt.Should().BeBefore(DateTime.UtcNow);

            account.IsActive = null;
            account.IsActive.Should().BeNull();

            account.IsActive = true;
            account.IsActive.Should().BeTrue();

            account.IsActive = false;
            account.IsActive.Should().BeFalse();
        }
    }
}