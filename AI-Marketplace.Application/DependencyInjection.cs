using Microsoft.Extensions.DependencyInjection;

namespace AI_Marketplace.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            
            services.AddMediatR(config => 
                config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);


            // --- Example: Add FluentValidation ---
            // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


            return services;
        }
    }
}