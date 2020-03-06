using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CacheServer.Interfaces;
using IdentityServer.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Api.Security
{
    public class TokenHelper : ITokenHelper
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ICacheRepository _cacheRepository;

        public TokenHelper(JwtOptions jwtOptions, ICacheRepository cacheRepository)
        {
            _jwtOptions = jwtOptions;
            _cacheRepository = cacheRepository;
        }

        public TokenResponse GenerateToken(Account account, string refreshToken = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

            var pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("Id", account.Id.ToString()));
            pairs.Add(new KeyValuePair<string, string>("Username", account.Username));
            pairs.Add(new KeyValuePair<string, string>("Email", account.UserMail));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.ValidIssuer,
                Audience = _jwtOptions.ValidAudience,
                Subject = GenerateClaimsIdentity(pairs.ToArray()),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresAfterMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            if (refreshToken == null)
            {
                refreshToken = Guid.NewGuid().ToString();
            }

            var tokenResponse = new TokenResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresIn = 60 * _jwtOptions.ExpiresAfterMinutes,
                RefreshToken = refreshToken
            };
            _cacheRepository.SetAsync(refreshToken, new TokenCache
            {
                Username = account.Username,
                Token = tokenResponse.AccessToken
            });

            return tokenResponse;
        }


        public JwtSecurityToken ValidateAndDecode(string token)
        {
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,

                ValidateIssuerSigningKey = true,

                ValidIssuer = _jwtOptions.ValidIssuer,
                ValidAudience = _jwtOptions.ValidAudience,

                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            try

            {
                new JwtSecurityTokenHandler().ValidateToken(
                    token, validationParameters, out var rawValidatedToken);
                return (JwtSecurityToken) rawValidatedToken;
            }
            catch (SecurityTokenValidationException securityTokenValidationException)
            {
                throw new Exception($"Token failed validation: {securityTokenValidationException.Message}");
            }
            catch (ArgumentException argumentException)
            {
                throw new Exception($"Token was invalid:{argumentException.Message}");
            }
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