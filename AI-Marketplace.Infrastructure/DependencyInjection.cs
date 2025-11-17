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
            // --- Example: Add EF Core ---
            // 1. You would first install:
            //    - Microsoft.EntityFrameworkCore.SqlServer
            //    - Microsoft.EntityFrameworkCore.Tools
            //
            // 2. You would add your connection string to appsettings.json in the WebAPI project.
            //
            // 3. You would uncomment and update this:
            /*
            services.AddDbContext<YourAppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            */


            // --- Example: Add Repositories ---
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<IProductRepository, ProductRepository>();


            // Add other infrastructure services (email, payments, etc.)
            // services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}