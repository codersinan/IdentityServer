using FluentAssertions;
using IdentityServer.Infrastructure.Mappings;
using NUnit.Framework;

namespace IdentityServer.Infrastructure.Tests.Mappings
{
    [TestFixture]
    public class SignUpRequestMappingTests
    {
        [Test]
        public void SignUpRequestMappingTest()
        {
            var mapping = new SignUpRequestMapping();
            mapping.Should().NotBeNull();
        }
    }
}