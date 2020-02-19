using IdentityServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Username).IsRequired();
            builder.Property(e => e.UserMail).IsRequired();
            builder.Property(e => e.PasswordSalt).IsRequired();
            builder.Property(e => e.PasswordHash).IsRequired();

            builder.HasAlternateKey(e => new {e.Username, e.UserMail});
        }
    }
}