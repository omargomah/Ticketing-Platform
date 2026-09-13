using Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class ApplicationRegistrations
    {
        public static IServiceCollection AddApplicationRegistrations(this IServiceCollection services)
        {
            Assembly assemblyOfApplication = typeof(ApplicationRegistrations).Assembly;
            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(assemblyOfApplication); });
            services.AddValidatorsFromAssembly(assemblyOfApplication);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviors<,>));
            return services;
        }
    }
}
