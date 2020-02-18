using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Core.Entities;
using IdentityServer.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data
{
    public class IdentityServerContext : DbContext
    {
        public IdentityServerContext(DbContextOptions<IdentityServerContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountConfiguration).Assembly);
        }

        public override int SaveChanges()
        {
            ChangeTrackerInterceptor();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTrackerInterceptor();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ChangeTrackerInterceptor()
        {
            var entries = ChangeTracker.Entries().Where(x =>
                x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        ((BaseEntity) entry.Entity).CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        ((BaseEntity) entry.Entity).LastModifiedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}