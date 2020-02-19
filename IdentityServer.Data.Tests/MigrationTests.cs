using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NUnit.Framework;

namespace IdentityServer.Data.Tests
{
    [TestFixture]
    public class MigrationTests
    {
        [Test]
        public void MigrationUpAndDownTest()
        {
            var context = new IdentityServerContextFactory().CreateDbContext(new string[] {"InMemoryDatabase"});
            MigrationBuilder builder = new MigrationBuilder(context.Database.ProviderName);

            var migrations = new List<Migration>()
            {
                new AccountTable(),
                new AccountTableAddActivationTokenColumn()
            };

            foreach (var migration in migrations)
            {
                migration.TargetModel.GetDefaultSchema();
                migration.TargetModel.GetEntityTypes();

                builder.Operations.AddRange(migration.UpOperations.ToList());
            }

            builder.Operations.Count.Should().BeGreaterThan(0);
            migrations.ToList().Reverse();

            builder.Operations.Clear();
            ;
            foreach (var migration in migrations)
            {
                builder.Operations.AddRange(migration.DownOperations.ToList());
            }

            builder.Operations.Count.Should().BeGreaterThan(0);
        }
    }
}