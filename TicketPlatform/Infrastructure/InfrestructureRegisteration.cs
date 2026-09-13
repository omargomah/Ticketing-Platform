using Application.IServices;
using Infrastructure.Identity;
using Infrastructure.Persistence.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            //services.AddIdentityCore<AppUser>(options =>
            //{
            //    // Password settings
            //    options.Password.RequiredLength = 8;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequireLowercase = true;
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireNonAlphanumeric = true;

            //    // User settings
            //    options.User.RequireUniqueEmail = true;

            //    // Lockout settings
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //    options.Lockout.MaxFailedAccessAttempts = 5;
            //    options.Lockout.AllowedForNewUsers = true;

            //    // Sign-in settings
            //    options.SignIn.RequireConfirmedEmail = true;
            //})
            //.AddRoles<IdentityRole<Guid>>() 
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders(); 
            
            services.AddScoped<IIdentityService,IdentityService>();
            
            return services;
        }
    }
}
