using System;

namespace IdentityServer.Infrastructure.ResponseModels
{
    public class CurrentAccount
    {
        public string Username { get; set; }
        public string Email{ get; set; }
        public DateTime CreatedAt { get; set; }
    }
}