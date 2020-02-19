namespace IdentityServer.Infrastructure.RequestModels
{
    public class SignUpRequest
    {
        public string Username { get; set; }
        public string UserMail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}