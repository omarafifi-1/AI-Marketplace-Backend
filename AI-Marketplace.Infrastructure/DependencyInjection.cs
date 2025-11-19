using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Infrastructure.Data;
using AI_Marketplace.Infrastructure.Repositories.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI_Marketplace.Infrastructure
{
    public static class DependencyInjection
    {
        // This 'this IServiceCollection services' part is what makes it an extension method
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
           
            // Register Repositories
            services.AddScoped<IStoreRepository, StoreRepository>();

            // Add other infrastructure services (email, payments, etc.)
            // services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}