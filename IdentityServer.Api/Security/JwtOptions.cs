namespace IdentityServer.Api.Security
{
    public class JwtOptions
    {
        public string SecretKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public int ExpiresAfterMinutes { get; set; }
    }
}