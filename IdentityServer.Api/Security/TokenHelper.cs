using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityServer.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Api.Security
{
    public class TokenHelper : ITokenHelper
    {
        private readonly JwtOptions _options;

        public TokenHelper(JwtOptions options)
        {
            _options = options;
        }

        public TokenResponse GenerateToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_options.SecretKey);

            var pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("Id", account.Id.ToString()));
            pairs.Add(new KeyValuePair<string, string>("Username", account.Username));
            pairs.Add(new KeyValuePair<string, string>("Email", account.UserMail));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.ValidIssuer,
                Audience = _options.ValidAudience,
                Subject = GenerateClaimsIdentity(pairs.ToArray()),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_options.ExpiresAfterMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenResponse {AccessToken = tokenHandler.WriteToken(token)};
        }

        private ClaimsIdentity GenerateClaimsIdentity(params KeyValuePair<string, string>[] values)
        {
            var claims = new ClaimsIdentity();
            foreach (KeyValuePair<string, string> keyValuePair in values)
            {
                claims.AddClaim(new Claim(keyValuePair.Key, keyValuePair.Value));
            }

            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            return claims;
        }
    }
}