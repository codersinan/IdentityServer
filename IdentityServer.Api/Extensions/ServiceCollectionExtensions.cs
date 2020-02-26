using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using FluentValidation.AspNetCore;
using IdentityServer.Api.Helpers;
using IdentityServer.Api.Security;
using IdentityServer.Data;
using IdentityServer.Infrastructure.Interfaces;
using IdentityServer.Infrastructure.Mappings;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Infrastructure.RequestModels;
using MailSender;
using MailSender.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace IdentityServer.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHealthChecksConfiguration(this IServiceCollection services)
        {
            services.AddHealthChecks().AddDbContextCheck<IdentityServerContext>();
        }

        public static void AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("IdentityServer",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
        }

        public static void AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
        }

        public static void AddControllersConfiguration(this IServiceCollection services)
        {
            services.AddControllers()
                // Configure Json Result
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssembly(typeof(SignUpRequest).Assembly);
                });
        }

        public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityServerContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("IdentityServer"));
                options.EnableSensitiveDataLogging(true);
            });
        }

        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(SignUpRequestMapping).Assembly);
        }

        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Identity Server",
                    Version = "v1"
                });

                options.SchemaFilter<SwaggerFluentValidationSchemaFilter>();

                #region Swagger Security

                // TODO doesn't work correct.It has authentication problem doesn't send authorization header correctly
                var securitySchema = new OpenApiSecurityScheme
                {
                    Name = "Authentication",
                    Description = "Identity Server Authentication",
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                };
                options.AddSecurityDefinition("Bearer", securitySchema);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string> { }
                    }
                });

                #endregion


                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        public static void AddAuthenticationConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtOptions = new JwtOptions();
            configuration.Bind(nameof(JwtOptions), jwtOptions);

            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

            services.AddSingleton(jwtOptions);
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtOptions.ValidIssuer,
                        ValidAudience = jwtOptions.ValidAudience,

                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
            services.AddTransient<ITokenHelper, TokenHelper>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IAccountRepository, AccountRepository>();
        }
        
        public static void AddMailServerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var mailConfiguration = new MailConfiguration();
            configuration.Bind("MailConfiguration", mailConfiguration);
            services.AddSingleton<IMailConfiguration>(mailConfiguration);

            services.AddTransient<IEMailService, CustomMailService>();
        }
    }
}