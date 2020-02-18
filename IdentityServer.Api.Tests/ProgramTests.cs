using FluentAssertions;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace IdentityServer.Api.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private IHostBuilder _hostBuilder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _hostBuilder = Program.CreateHostBuilder(new string[]{});
        }

        [Test]
        public void ProgramCreateIHostBuilderTest()
        {
            _hostBuilder.Should().NotBeNull();
            var host = _hostBuilder.Build();
            host.Should().NotBeNull();
        }
    }
}