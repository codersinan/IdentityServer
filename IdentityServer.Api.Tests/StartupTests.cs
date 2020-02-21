using System.IO;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;

namespace IdentityServer.Api.Tests
{
    [TestFixture]
    public class StartupTests
    {
        private TestServer _server;
        private IWebHostEnvironment _environment;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _environment = new WebHostEnvironment
            {
                ApplicationName = string.Empty,
                EnvironmentName = string.Empty,
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = Directory.GetCurrentDirectory(),
                ContentRootFileProvider = new NullFileProvider(),
                WebRootFileProvider = new NullFileProvider(),
            };

            var settings = "appsettings";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{settings}.json")
                .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();

            _server = new TestServer(webHostBuilder);
        }

        [Test]
        public void StartupCheckTestServer()
        {
            _server.Should().NotBeNull();
        }

        [Test]
        public void GetConfiguration()
        {
            var startup = new Startup(_environment);

            startup.Configuration.Should().NotBeNull();
        }
    }

    public class WebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
    }
}