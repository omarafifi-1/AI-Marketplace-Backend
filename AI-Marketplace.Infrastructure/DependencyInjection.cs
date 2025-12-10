using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Common.Settings;
using AI_Marketplace.Infrastructure.Data;
using AI_Marketplace.Infrastructure.ExternalServices;
using AI_Marketplace.Infrastructure.ExternalServices.payment;
using AI_Marketplace.Infrastructure.Repositories.Addresses;
using AI_Marketplace.Infrastructure.Repositories.Cart;
using AI_Marketplace.Infrastructure.Repositories.Categories;
using AI_Marketplace.Infrastructure.Repositories.CustomRequests;
using AI_Marketplace.Infrastructure.Repositories.Offers;
using AI_Marketplace.Infrastructure.Repositories.Orders;
using AI_Marketplace.Infrastructure.Repositories.Payments;
using AI_Marketplace.Infrastructure.Repositories.Products;
using AI_Marketplace.Infrastructure.Repositories.Stores;
using AI_Marketplace.Infrastructure.Repositories.Wishlist;
using AI_Marketplace.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;


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
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICustomRequestRepository, CustomRequestRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();

            // Register JWT Token Service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Register File Service
            services.AddScoped<IFileService, Services.FileService>();

            // Stripe
            services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));

            // Register a singleton StripeClient using the SecretKey from configuration
            services.AddSingleton<StripeClient>(sp =>
            {
                var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<StripeOptions>>().Value;
                return new StripeClient(opts.SecretKey);
            });

            // Register Stripe SDK services that StripePaymentService depends on
            services.AddScoped<PaymentIntentService>(sp =>
            {
                var client = sp.GetRequiredService<StripeClient>();
                return new PaymentIntentService(client);
            });
            services.AddScoped<RefundService>(sp =>
            {
                var client = sp.GetRequiredService<StripeClient>();
                return new RefundService(client);
            });

            // Register your abstraction
            services.AddScoped<IStripePaymentService, StripePaymentService>();

            // Register Email Service
            services.AddTransient<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}