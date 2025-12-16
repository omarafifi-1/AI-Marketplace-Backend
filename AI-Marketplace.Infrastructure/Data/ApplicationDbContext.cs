using AI_Marketplace.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AI_Marketplace.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //Corresponding table entities inside my database 
        public DbSet<Store> Stores { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CustomRequest> CustomRequests { get; set; }
        public DbSet<GeneratedImage> GeneratedImages { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<MasterOrder> MasterOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Cart> Carts { get; set; } 
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store Configuration
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StoreName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Rating).HasPrecision(3, 2);

                entity.HasOne(e => e.Owner)
                    .WithOne(u => u.Store)
                    .HasForeignKey<Store>(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Rating).HasPrecision(3, 2);

                entity.HasOne(e => e.Store)
                    .WithMany(s => s.Products)
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ProductImage Configuration
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CustomRequest Configuration
            modelBuilder.Entity<CustomRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasConversion<string>();

                entity.HasOne(e => e.Buyer)
                    .WithMany(u => u.CustomRequests)
                    .HasForeignKey(e => e.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.CustomRequests)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                // add ImageUrl,Budget,Deadline properties
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Budget).HasPrecision(18, 2);       
                entity.Property(e => e.Deadline).HasColumnType("datetime2");
            });

            // GeneratedImage Configuration
            modelBuilder.Entity<GeneratedImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Prompt).IsRequired().HasMaxLength(2000);

                entity.HasOne(e => e.CustomRequest)
                    .WithMany(cr => cr.GeneratedImages)
                    .HasForeignKey(e => e.CustomRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Offer Configuration
            modelBuilder.Entity<Offer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProposedPrice).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasIndex(e => new { e.CustomRequestId, e.StoreId })
                      .IsUnique();

                entity.HasOne(e => e.CustomRequest)
                    .WithMany(cr => cr.Offers)
                    .HasForeignKey(e => e.CustomRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Store)
                    .WithMany(s => s.Offers)
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MasterOrder Configuration
            modelBuilder.Entity<MasterOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(e => e.Buyer)
                    .WithMany()
                    .HasForeignKey(e => e.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ShippingAddressEntity)
                    .WithMany()
                    .HasForeignKey(e => e.ShippingAddressId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // MasterOrder Configuration
            modelBuilder.Entity<MasterOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(e => e.Buyer)
                    .WithMany()
                    .HasForeignKey(e => e.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ShippingAddressEntity)
                    .WithMany()
                    .HasForeignKey(e => e.ShippingAddressId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(e => e.Buyer)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Offer)
                    .WithOne(o => o.Order)
                    .HasForeignKey<Order>(e => e.OfferId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ShippingAddressEntity)
                    .WithMany(a => a.Orders)
                    .HasForeignKey(e => e.ShippingAddressId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.MasterOrder)
                    .WithMany(mo => mo.ChildOrders)
                    .HasForeignKey(e => e.MasterOrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Review Configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comment).HasMaxLength(1000);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Store)
                    .WithMany(s => s.Reviews)
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ChatSession Configuration
            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.ChatSessions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatMessage Configuration
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Content).IsRequired();

                entity.HasOne(e => e.ChatSession)
                    .WithMany(cs => cs.ChatMessages)
                    .HasForeignKey(e => e.ChatSessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Cart)
                    .HasForeignKey<Cart>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId)
                    .IsUnique(); 

                entity.Ignore(e => e.TotalAmount); 
                entity.Ignore(e => e.TotalItems);  
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UnitPrice)
                    .HasPrecision(18, 2);

                entity.HasOne(e => e.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(e => e.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate products in same cart
                entity.HasIndex(e => new { e.CartId, e.ProductId })
                    .IsUnique();

                entity.Ignore(e => e.TotalPrice); // Calculated property
            });

            // Address configuration
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Street).IsRequired().HasMaxLength(500);
                entity.Property(e => e.City).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(200);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

               

                entity.HasIndex(e => new { e.UserId, e.IsPrimary }).HasFilter(null);
            });

            // Payment Configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasConversion<string>();
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>();
                
                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(3);
                
                entity.Property(e => e.PaymentIntentId)
                    .HasMaxLength(255);
                
                entity.Property(e => e.TransactionId)
                    .HasMaxLength(255);
                
                entity.Property(e => e.RefundTransactionId)
                    .HasMaxLength(255);
                
                entity.Property(e => e.CustomerEmail)
                    .HasMaxLength(256);
                
                entity.Property(e => e.CustomerName)
                    .HasMaxLength(200);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Payments)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.MasterOrder)
                    .WithMany(mo => mo.Payments)
                    .HasForeignKey(e => e.MasterOrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PaymentIntentId);
                entity.HasIndex(e => e.TransactionId);
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.MasterOrderId);
            });

            // Wishlist Configuration
            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Wishlists)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Wishlists)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate products in same user's wishlist
                entity.HasIndex(e => new { e.UserId, e.ProductId })
                    .IsUnique();
            });
        }
    }
}
