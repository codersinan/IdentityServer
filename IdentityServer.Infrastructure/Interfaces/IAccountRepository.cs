using System.Threading.Tasks;
using IdentityServer.Core.Entities;

namespace IdentityServer.Infrastructure.Interfaces
{
    public interface IAccountRepository
    {
        Account SignUp(Account account);
        Task<Account> SignUpAsync(Account account);
    }
}