using System;
using FluentAssertions;
using IdentityServer.Core.Entities;
using IdentityServer.Data;
using IdentityServer.Infrastructure.Interfaces;
using IdentityServer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IdentityServer.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class AccountRepositoryTests
    {
        private ServiceProvider _serviceProvider;
        private IAccountRepository _accountRepository;

        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<IdentityServerContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
            serviceCollection.AddTransient<IAccountRepository, AccountRepository>();
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _accountRepository = _serviceProvider.GetService<IAccountRepository>();
        }

        [Test]
        public void SignUpAccountIsNull()
        {
            // arrange
            Account request = null;
            // act

            // assert
            _accountRepository
                .Invoking(m => m.SignUp(request))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void SignUpAsyncAccountIsNull()
        {
            // arrange
            Account request = null;
            // act

            // assert
            _accountRepository
                .Invoking(m => m.SignUpAsync(request))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public void SignUpWithAllParametersNull()
        {
            // arrange
            var request = new Account
            {
            };
            // act

            // assert
            _accountRepository
                .Invoking(m => m.SignUp(request))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void SignUpAsyncWithAllParametersNull()
        {
            // arrange
            var request = new Account
            {
            };
            // act

            // assert
            _accountRepository
                .Invoking(m => m.SignUpAsync(request))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public void SignUpWithAllParametersAreCorrect()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            // act

            // assert
            _accountRepository.Invoking(m => m.SignUp(request)).Invoke()
                .Should().NotBeNull();
        }

        [Test]
        public void SignUpAsyncWithAllParametersAreCorrect()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            // act

            // assert
            _accountRepository.Invoking(m => m.SignUpAsync(request)).Invoke()
                .Should().NotBeNull();
        }

        [Test(Description = "Username is already used")]
        public void SignUpWithAllParametersAreCorrectButAlreadyUsedUsernameOrUserMail()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "b@b.com",
                PasswordHash = "123"
            };
            // act
            _accountRepository
                .Invoking(m=>m.SignUp(new Account {Username = "a", UserMail = "a@a.com", PasswordHash = "123"}))
                .Should().NotThrow();
            // assert
            _accountRepository.Invoking(m => m.SignUp(request))
                .Should().Throw<InvalidOperationException>();
        }
        
        [Test(Description = "UserMail is already used")]
        public void SignUpAsyncWithAllParametersAreCorrectButAlreadyUsedUsernameOrUserMail()
        {
            // arrange
            var request = new Account()
            {
                Username = "b",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            // act
            _accountRepository
                .Invoking(m=>m.SignUpAsync(new Account {Username = "b", UserMail = "a@a.com", PasswordHash = "123"}))
                .Should().NotThrow();
            // assert
            _accountRepository.Invoking(m => m.SignUpAsync(request))
                .Should().ThrowAsync<InvalidOperationException>();
        }
    }
}