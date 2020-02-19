namespace IdentityServer.Core.Entities
{
    public class Account : BaseEntity
    {
        public string Username { get; set; }
        public string UserMail { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public bool? IsActive { get; set; }
    }
}