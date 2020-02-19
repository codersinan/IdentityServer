using FluentAssertions;
using IdentityServer.Infrastructure.Mappings;
using NUnit.Framework;

namespace IdentityServer.Infrastructure.Tests.Mappings
{
    [TestFixture]
    public class SignInRequestMappingTests
    {
        [Test]
        public void SignUpRequestMappingTest()
        {
            var mapping = new SignInRequestMapping();
            mapping.Should().NotBeNull();
        }
    }
}