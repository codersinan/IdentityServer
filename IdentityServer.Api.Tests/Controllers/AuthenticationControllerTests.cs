using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using IdentityServer.Api.Security;
using IdentityServer.Core.Entities;
using IdentityServer.Data;
using IdentityServer.Infrastructure.Interfaces;
using IdentityServer.Infrastructure.RequestModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace IdentityServer.Api.Tests.Controllers
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private string controllerPath = "api/authentication";

        private TestServer _server;
        private HttpClient _client;
        private IdentityServerContext _context;
        private IAccountRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var guid = Guid.NewGuid().ToString();
            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("Test")
                .UseKestrel()
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.RemoveAll<IdentityServerContext>();
                    services.AddDbContext<IdentityServerContext>(options =>
                    {
                        options.UseInMemoryDatabase(guid);
                    });
                })
                .ConfigureServices(services =>
                {
                    services.RemoveAll<IdentityServerContext>();
                    services.AddDbContext<IdentityServerContext>(options =>
                    {
                        options.UseInMemoryDatabase(guid);
                    });
                });

            _server = new TestServer(webHostBuilder);

            _context = _server.Host.Services.GetService<IdentityServerContext>();
            
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = _server.Host.Services.GetService<IAccountRepository>();

            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5100");
        }

        [Test]
        public void SignUpTestWithInvalidParameters()
        {
            // arrange
            var request = new SignUpRequest
            {
                Username = "a",
                UserMail = "a",
                Password = "1",
                ConfirmPassword = "123"
            };

            // act
            var response = _client.PostAsync(
                $"{controllerPath}/SignUp",
                request.ToHttpContent()
            ).Result;

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public void SignUpTestWithValidParameters()
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
            var response = _client.PostAsync(
                $"{controllerPath}/signup",
                request.ToHttpContent()
            ).Result;

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void SignUpTestWithValidParametersButAlreadyUsedUsername()
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
            var response = _client.PostAsync(
                $"{controllerPath}/signup",
                request.ToHttpContent()
            ).Result;

            response = _client.PostAsync(
                $"{controllerPath}/signup",
                request.ToHttpContent()
            ).Result;
            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public void CheckActivationTokenTestWithNotGuid()
        {
            // arrange

            // act
            var response = _client.GetAsync($"{controllerPath}/Activation/{1}").Result;

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public void CheckActivationTokenTestWithWrongGuid()
        {
            // arrange

            // act
            var response = _client.GetAsync($"{controllerPath}/Activation/{Guid.NewGuid()}").Result;

            // assert

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public void CheckActivatioTestTokenWithGuid()
        {
            // arrange
            var account = new Account
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };

            // act
            _repository.SignUp(account);
            Guid activationToken =
                _context.Accounts.FirstOrDefault(x => x.Username == account.Username).ActivationToken;

            // assert

            var response = _client.GetAsync(
                $"{controllerPath}/Activation/{activationToken}"
            ).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ActivateAccountTestWithInvalidToken()
        {
            // arrange

            // act

            // assert
            var response = _client.PostAsync(
                $"{controllerPath}/Activation/{Guid.NewGuid()}", null
            ).Result;
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public void ActivateAccountTestWithValidToken()
        {
            // arrange
            var account = new Account
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };

            // act
            _repository.SignUpAsync(account);
            Guid activationToken =
                _context.Accounts.FirstOrDefault(x => x.Username == account.Username).ActivationToken;

            // assert
            var response = _client.PostAsync(
                $"{controllerPath}/Activation/{activationToken}", null
            ).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void SignInTestWithWrongInformation()
        {
            // arrange
            var request = new SignInRequest
            {
                Username = "a",
                Password = "1"
            };
            // act

            // assert
            var response = _client.PostAsync(
                $"{controllerPath}/SignIn", request.ToHttpContent()
            ).Result;
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public void SignInTestWithWrongPassword()
        {
            // arrange
            var request = new SignInRequest
            {
                Username = "a",
                Password = Guid.NewGuid().ToString()
            };

            // act
            RegisterAccountAndActivate();
            // assert
            var response = _client.PostAsync(
                $"{controllerPath}/SignIn", request.ToHttpContent()
            ).Result;
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void SignInTestWithValidParameters()
        {
            // arrange

            var request = new SignInRequest
            {
                Username = "a",
                Password = "123"
            };

            // act
            RegisterAccountAndActivate();
            // assert
            var response = _client.PostAsync(
                $"{controllerPath}/SignIn", request.ToHttpContent()
            ).Result;
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tokenResponse = response.ToObject<TokenResponse>();
            tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CurrentAccountTestWithoutAuthorizationHeader()
        {
            // arrange
            var request = new SignInRequest
            {
                Username = "a",
                Password = "123"
            };
            // act
            RegisterAccountAndActivate();

            var response = _client.PostAsync(
                $"{controllerPath}/SignIn", request.ToHttpContent()
            ).Result;
            var token = response.ToObject<TokenResponse>().AccessToken;

            response = _client.GetAsync($"{controllerPath}/Account").Result;

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void CurrentAccountTestWithAuthorizationToken()
        {
            // arrange
            var request = new SignInRequest
            {
                Username = "a",
                Password = "123"
            };
            // act
            RegisterAccountAndActivate();

            var response = _client.PostAsync(
                $"{controllerPath}/SignIn", request.ToHttpContent()
            ).Result;
            var token = response.ToObject<TokenResponse>().AccessToken;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            response = _client.GetAsync($"{controllerPath}/Account").Result;


            // assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private void RegisterAccountAndActivate()
        {
            var account = new Account
            {
                Username = "a",
                UserMail = "a@a.com",
                PasswordHash = "123"
            };
            _repository.SignUpAsync(account);
            Guid activationToken =
                _context.Accounts.FirstOrDefault(x => x.Username == account.Username).ActivationToken;
            _client.PostAsync(
                $"{controllerPath}/Activation/{activationToken}", null
            );
        }
    }
}