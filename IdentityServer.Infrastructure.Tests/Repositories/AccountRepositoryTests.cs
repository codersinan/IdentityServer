using System;
using System.Linq;
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
                .Invoking(m => m.SignUp(new Account {Username = "a", UserMail = "a@a.com", PasswordHash = "123"}))
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
                .Invoking(m => m.SignUpAsync(new Account {Username = "b", UserMail = "a@a.com", PasswordHash = "123"}))
                .Should().NotThrow();
            // assert
            _accountRepository.Invoking(m => m.SignUpAsync(request))
                .Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        public void CheckActivationTestTokenWithInvalidToken()
        {
            // arrange

            // act

            // assert
            _accountRepository.Invoking(m => m.CheckActivationToken(Guid.Empty)).Invoke().Should()
                .BeFalse();
        }

        [Test]
        public void CheckActivationTokenTestWithValidToken()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            var context = _serviceProvider.GetService<IdentityServerContext>();
            Account account = null;
            // act
            _accountRepository
                .Invoking(m => m.SignUp(request))
                .Should().NotThrow();

            account = context.Accounts.FirstOrDefault(x => x.Username == request.Username);
            // assert

            account.Should().NotBeNull();
            _accountRepository
                .Invoking(m => m.CheckActivationToken(account.ActivationToken))
                .Invoke().Should().BeTrue();
        }

        [Test]
        public void ActivateAccountTestWithInvalidToken()
        {
            // arrange

            // act

            // assert
            _accountRepository
                .Invoking(m => m.ActivateAccount(Guid.Empty))
                .Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void ActivateAccountTestWithValidToken()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            var context = _serviceProvider.GetService<IdentityServerContext>();
            Account account = null;
            // act
            _accountRepository
                .Invoking(m => m.SignUp(request))
                .Should().NotThrow();

            account = context.Accounts.FirstOrDefault(x => x.Username == request.Username);
            // assert

            account.Should().NotBeNull();
            _accountRepository
                .Invoking(m => m.ActivateAccount(account.ActivationToken))
                .Should().NotThrow();
        }

        [Test]
        public void SignInTestWithNotRegisteredAccount()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                PasswordHash = "123"
            };

            // act

            // assert
            _accountRepository
                .Invoking(m => m.SignIn(request))
                .Invoke()
                .Should().BeNull();
        }

        [Test]
        public void SignInTestWithRegisteredAccountButWrongPassword()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            // act
            _accountRepository.SignUp(request);
            request.PasswordHash = "124";
            // assert
            _accountRepository
                .Invoking(m => m.SignIn(request))
                .Invoke()
                .Should().BeNull();
        }

        [Test]
        public void SignInTestWithRegisteredAccount()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            // act
            _accountRepository.SignUp(request);// this command write request variable with created account
            _accountRepository.ActivateAccount(request.ActivationToken);
            
            // request changed for 
            request=new Account
            {
                Username = "a",
                PasswordHash = "123"
            };
            // assert
            _accountRepository
                .Invoking(m => m.SignIn(request))
                .Invoke()
                .Should().NotBeNull();
        }
        
        [Test]
        public void SignInAsyncTestWithNotRegisteredAccount()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                PasswordHash = "123"
            };

            // act

            // assert
            _accountRepository
                .Invoking(m => m.SignInAsync(request))
                .Invoke().Result
                .Should().BeNull();
        }

        [Test]
        public void SignInAsyncTestWithRegisteredAccountButWrongPassword()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            // act
            _accountRepository.SignUpAsync(request).ConfigureAwait(true);
            request.PasswordHash = "124";
            // assert
            _accountRepository
                .Invoking(m => m.SignInAsync(request))
                .Invoke().Result
                .Should().BeNull();
        }

        [Test]
        public void SignInAsyncTestWithRegisteredAccount()
        {
            // arrange
            var request = new Account()
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            // act
            _accountRepository.SignUp(request);// this command write request variable with created account
            _accountRepository.ActivateAccount(request.ActivationToken);
            
            // request changed for 
            request=new Account
            {
                Username = "a",
                PasswordHash = "123"
            };
            // assert
            _accountRepository
                .Invoking(m => m.SignInAsync(request))
                .Invoke().Result
                .Should().NotBeNull();
        }
    }
}