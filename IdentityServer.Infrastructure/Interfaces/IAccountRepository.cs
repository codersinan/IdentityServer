using System;
using System.Threading.Tasks;
using IdentityServer.Core.Entities;

namespace IdentityServer.Infrastructure.Interfaces
{
    public interface IAccountRepository
    {
        Account SignUp(Account account);
        Task<Account> SignUpAsync(Account account);

        bool CheckActivationToken(Guid token);
        void ActivateAccount(Guid token);
    }
}