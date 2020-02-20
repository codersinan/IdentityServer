using IdentityServer.Core.Entities;

namespace IdentityServer.Api.Security
{
    public interface ITokenHelper
    {
        public TokenResponse GenerateToken(Account account);
    }
}