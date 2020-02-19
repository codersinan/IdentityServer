using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using IdentityServer.Api.Controllers;
using IdentityServer.Api.Extensions;
using IdentityServer.Core.Entities;
using IdentityServer.Data;
using IdentityServer.Infrastructure.Interfaces;
using IdentityServer.Infrastructure.Mappings;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Infrastructure.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IdentityServer.Api.Tests.Controllers
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private AuthenticationController _controller;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<IdentityServerContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            serviceCollection.AddAutoMapper(typeof(SignUpRequestMapping).Assembly);
            serviceCollection.AddTransient<IAccountRepository, AccountRepository>();
            serviceCollection.AddControllersConfiguration();
            _serviceProvider = serviceCollection.BuildServiceProvider();

            var repository = _serviceProvider.GetService<IAccountRepository>();
            var mapper = _serviceProvider.GetService<IMapper>();

            _controller = new AuthenticationController(repository, mapper);
        }

        [Test]
        public void SignUpWithNewControllerWithoutConstructorParameters()
        {
            // arrange
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a@a.com",
                Password = "123",
                ConfirmPassword = "123"
            };

            var controller = new AuthenticationController(null, null);
            // act

            // assert
            controller.SignUp(request).Result
                .Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void SignUpWithInvalidParameters()
        {
            // arrange
            var request = new SignUpRequest
            {
            };
            // act
            _controller.ModelState.AddModelError("Exception", "Error");
            // assert
            _controller.SignUp(request).Result
                .Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void SignUpWithValidParameters()
        {
            // arrange
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a@a.com",
                Password = "123",
                ConfirmPassword = "123"
            };
            // act

            // assert
            _controller
                .Invoking(m => m.SignUp(request))
                .Invoke().Result
                .Should().BeOfType<OkResult>();
        }

        [Test]
        public void CheckActivationTokenWithInvalidToken()
        {
            // arrange

            // act

            // assert
            _controller
                .Invoking(m => m.CheckActivationToken(string.Empty))
                .Invoke()
                .Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void CheckActivationTokenWithValidToken()
        {
            // arrange
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a@a.com",
                Password = "123",
                ConfirmPassword = "123"
            };
            var context = _serviceProvider.GetService<IdentityServerContext>();
            Account account = null;
            // act
            _controller
                .Invoking(m => m.SignUp(request))
                .Invoke();
            account = context.Accounts.FirstOrDefault(x => x.Username == request.Username);
            // assert
            _controller
                .Invoking(m => m.CheckActivationToken(account.ActivationToken.ToString()))
                .Invoke()
                .Should().BeOfType<OkResult>();
        }

        [Test]
        public void ActivateAccountWithInvalidToken()
        {
            // arrange

            // act

            // assert
            _controller
                .Invoking(m => m.ActivateAccount(string.Empty))
                .Invoke()
                .Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ActivateAccountWithValidToken()
        {
            // arrange
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a@a.com",
                Password = "123",
                ConfirmPassword = "123"
            };
            var context = _serviceProvider.GetService<IdentityServerContext>();
            Account account = null;
            // act
            _controller
                .Invoking(m => m.SignUp(request))
                .Invoke();
            account = context.Accounts.FirstOrDefault(x => x.Username == request.Username);
            // assert
            _controller
                .Invoking(m => m.ActivateAccount(account.ActivationToken.ToString()))
                .Invoke()
                .Should().BeOfType<OkResult>();
        }
    }
}