using Application.IServices;
using Infrastructure.Identity;
using Infrastructure.Options;
using Infrastructure.Persistence.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection AddInfrastructureRegisteration(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });


            services.AddIdentityCore<AppUser>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            #region Email Service Registeration  

            services.AddScoped<IEmailService, EmailService>();
            services.Configure<EmailOptions>(configuration.GetSection(nameof(EmailOptions)));

            #endregion
            
            #region Auth Registeration
            services.AddScoped<IIdentityService,IdentityService>();
            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            JwtOptions jwtConfiguration = configuration
            .GetSection(nameof(JwtOptions))
            .Get<JwtOptions>()!;
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfiguration.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey))
                };
            });
            #endregion


            return services;
        }
    }
}
