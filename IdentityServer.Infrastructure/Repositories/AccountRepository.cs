using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Core.Entities;
using IdentityServer.Data;
using IdentityServer.Infrastructure.Helpers;
using IdentityServer.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IdentityServerContext _context;

        public AccountRepository(IdentityServerContext context)
        {
            _context = context;
        }

        public Account SignUp(Account account)
        {
            CheckAccountIsNull(account);
            var exists = _context.Accounts.FirstOrDefault(x =>
                x.Username == account.Username || x.UserMail == account.UserMail);
            if (exists != null)
                throw new InvalidOperationException(nameof(account.Username) + " or " + nameof(account.UserMail) +
                                                    " already used");
            account = GenerateSaltAndHashPassword(account);
            account = GenerateActivationToken(account);

            _context.Set<Account>().Add(account);
            _context.SaveChanges();
            return account;
        }

        public async Task<Account> SignUpAsync(Account account)
        {
            CheckAccountIsNull(account);
            var exists = await _context.Accounts.FirstOrDefaultAsync(x =>
                x.Username == account.Username || x.UserMail == account.UserMail);
            if (exists != null)
                throw new InvalidOperationException(nameof(account.Username) + " or " + nameof(account.UserMail) +
                                                    " already used");
            account = GenerateSaltAndHashPassword(account);
            account = GenerateActivationToken(account);

            await _context.Set<Account>().AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public bool CheckActivationToken(Guid token)
        {
            var exits = _context.Accounts.FirstOrDefault(x => x.ActivationToken == token && x.IsActive == null);
            return exits != null;
        }

        public void ActivateAccount(Guid token)
        {
            var exits = _context.Accounts.FirstOrDefault(x => x.ActivationToken == token && x.IsActive == null);
            if (exits!=null)
            {
                exits.IsActive = true;
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException();
            }
            
        }

        private void CheckAccountIsNull(Account account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
        }

        private Account GenerateSaltAndHashPassword(Account account)
        {
            account.PasswordSalt = PasswordHelper.GenerateSalt();
            account.PasswordHash = PasswordHelper.HashPassword(account.PasswordSalt, account.PasswordHash);
            return account;
        }

        private Account GenerateActivationToken(Account account)
        {
            account.ActivationToken = Guid.NewGuid();
            return account;
        }
    }
}