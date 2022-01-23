using idb.Backend.DataAccess;
using idb.Backend.DataAccess.Repositories;
using idb.Backend.Middlewares;
using idb.Backend.Providers;
using idb.Backend.Services;
using idb.Backend.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System;
using System.Diagnostics.CodeAnalysis;

namespace idb.Backend
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        { }
        public void ConfigureServices(IServiceCollection services)
        {
            var enviormentProvider = new EnvironmentProvider();
            services.AddControllers();

            services.AddSingleton<IJwtTokenValidator, TokenValidator>();
            services.AddSingleton<IAuthJwtService, AuthService>();
            services.AddSingleton<IdbContext>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IJwtEnvironmentProvider>(enviormentProvider);
            services.AddSingleton<IDatabaseEnvironmentProvider>(enviormentProvider);
            services.AddSingleton<ISentryEnvironmentProvider>(enviormentProvider);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddSingleton<IMongoClient>(x =>
            {
                var conn = x.GetService<IDatabaseEnvironmentProvider>()?.DatabaseConnection;
                return new MongoClient(conn);
            });

            services.AddSentry();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "idb.Backend", Version = "v1" });
                OpenApiSecurityScheme securityDefinition = new()
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                };
                c.AddSecurityDefinition("jwt_auth", securityDefinition);

                // Make sure swagger UI requires a Bearer token specified
                OpenApiSecurityScheme securityScheme = new()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "jwt_auth",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                OpenApiSecurityRequirement securityRequirements = new()
                {
                    { securityScheme, Array.Empty<string>() },
                };
                c.AddSecurityRequirement(securityRequirements);
            });
            // stop being a smol dick CORSSS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "idb.Backend v1"));
            }

            app.UseCors();

            app.UseSentryTracing();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}