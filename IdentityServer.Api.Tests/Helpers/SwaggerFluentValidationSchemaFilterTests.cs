using FluentAssertions;
using IdentityServer.Api.Extensions;
using IdentityServer.Api.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IdentityServer.Api.Tests.Helpers
{
    [TestFixture]
    public class SwaggerFluentValidationSchemaFilterTests
    {
        private ServiceProvider _provider;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var services = new ServiceCollection();
            services.AddControllersConfiguration();
            services.AddSwaggerConfiguration();
            _provider = services.BuildServiceProvider();
        }

        [Test]
        public void ConstructorTest()
        {
            // arrange
            var schemaFilter = new SwaggerFluentValidationSchemaFilter(_provider);
            // act

            // assert
            schemaFilter.Should().NotBeNull();
        }
    }
}