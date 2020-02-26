using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.Data
{
    public class IdentityServerContextFactory : IDesignTimeDbContextFactory<IdentityServerContext>
    {
        public IdentityServerContext CreateDbContext(string[] args)
        {
            const string settings = "appsettings";
            // Get environment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            // Build config
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory().Replace("Data", "Api"))
                .AddJsonFile($"{settings}.json", optional: true)
                .AddJsonFile($"{settings}.{environment}.json", optional: true)
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<IdentityServerContext>();
            if (args.Length > 0 && args[0] == "InMemoryDatabase")
            {
                optionsBuilder.UseInMemoryDatabase("Data Source=:memory:");
            }
            else
            {
                optionsBuilder.UseNpgsql(config.GetConnectionString("IdentityServer"));
            }


            return new IdentityServerContext(optionsBuilder.Options);
        }
    }
}