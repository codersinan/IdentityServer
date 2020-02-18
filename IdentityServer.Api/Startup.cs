using IdentityServer.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment)
        {
            const string settings = "appsettings";
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(environment.ContentRootPath);

            builder
                .AddJsonFile($"{settings}.json", true, true)
                .AddJsonFile($"{settings}.{environment.EnvironmentName}.json", true, true);

            builder.AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecksConfiguration();

            services.AddCorsConfiguration();

            services.AddApiVersioningConfiguration();

            services.AddDbContextConfiguration(Configuration);
            
            services.AddControllersConfiguration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");
            app.UseCors("IdentityServer");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}