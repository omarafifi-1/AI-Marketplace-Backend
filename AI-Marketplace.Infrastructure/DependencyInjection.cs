using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Common.Settings;
using AI_Marketplace.Infrastructure.Data;
using AI_Marketplace.Infrastructure.ExternalServices;
using AI_Marketplace.Infrastructure.Repositories.Categories;
using AI_Marketplace.Infrastructure.Repositories.CustomRequests;
using AI_Marketplace.Infrastructure.Repositories.Offers;
using AI_Marketplace.Infrastructure.Repositories.Orders;
using AI_Marketplace.Infrastructure.Repositories.Products;
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
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICustomRequestRepository, CustomRequestRepository>();

            // Register JWT Token Service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Register File Service
            services.AddScoped<IFileService, Services.FileService>();

            return services;
        }
    }
}