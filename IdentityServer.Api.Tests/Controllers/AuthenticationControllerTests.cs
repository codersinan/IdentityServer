using System;
using AutoMapper;
using FluentAssertions;
using IdentityServer.Api.Controllers;
using IdentityServer.Api.Extensions;
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
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var repository = serviceProvider.GetService<IAccountRepository>();
            var mapper = serviceProvider.GetService<IMapper>();

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
            controller.SignUp(request).Result.Should().BeOfType<BadRequestObjectResult>();
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
            _controller.SignUp(request).Result.Should().BeOfType<BadRequestObjectResult>();
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
            _controller.Invoking(m => m.SignUp(request)).Invoke().Result
                .Should().BeOfType<OkResult>();
        }
    }
}