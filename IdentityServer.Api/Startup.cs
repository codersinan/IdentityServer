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

            services.AddAutoMapperConfiguration();

            services.AddControllersConfiguration();

            services.AddAuthenticationConfiguration(Configuration);

            services.AddSwaggerConfiguration();

            services.AddDbContextConfiguration(Configuration);

            services.AddMailServerConfiguration(Configuration);
            
            services.AddRepositories();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplyMigrations();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");
            app.UseCors("IdentityServer");

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Server");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}