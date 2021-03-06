using System.IdentityModel.Tokens.Jwt;
using IdentityServer.Core.Entities;

namespace IdentityServer.Api.Security
{
    public interface ITokenHelper
    {
        public TokenResponse GenerateToken(Account account,string refreshToken=null);
        public JwtSecurityToken ValidateAndDecode(string token);
    }
}