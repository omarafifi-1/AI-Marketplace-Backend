using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using System;
using System.Linq;

namespace AI_Marketplace.Infrastructure.Seed
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            Console.WriteLine("========== DB SEED START ==========");

            try
            {
                var seedOwner = context.Users.FirstOrDefault(u => u.Id == 7);
               

                // CATEGORIES
                Console.WriteLine($"Categories count: {context.Categories.Count()}");
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { Name = "Electronics" },
                        new Category { Name = "Clothing" },
                        new Category { Name = "Home & Kitchen" }
                    );
                    context.SaveChanges();
                }

                var electronics = context.Categories.FirstOrDefault(c => c.Name == "Electronics");
                var electronicsId = electronics?.Id ?? 0;
                Console.WriteLine($"Electronics ID = {electronicsId}");
                if (electronicsId == 0) { Console.WriteLine("ERROR: Electronics category not found!"); return; }

                var clothing = context.Categories.FirstOrDefault(c => c.Name == "Clothing");
                var clothingId = clothing?.Id ?? 0;
                Console.WriteLine($"Clothing ID = {clothingId}");
                if (clothingId == 0) { Console.WriteLine("ERROR: Clothing category not found!"); return; }

                var home = context.Categories.FirstOrDefault(c => c.Name == "Home & Kitchen");
                var homeId = home?.Id ?? 0;
                Console.WriteLine($"Home ID = {homeId}");
                if (homeId == 0) { Console.WriteLine("ERROR: Home & Kitchen category not found!"); return; }


                // PRODUCTS
                Console.WriteLine($"Products count: {context.Products.Count()}");
                var products = new List<Product>
                            {
                                // Electronics
                                new Product { Name = "Wireless Headphones", Description = "Bluetooth noise-cancelling headphones", Price = 150m, Stock = 20, CategoryId = electronicsId, StoreId = 3, IsActive = true },
                                new Product { Name = "Smartphone", Description = "Latest model smartphone with 128GB storage", Price = 699m, Stock = 15, CategoryId = electronicsId, StoreId = 3, IsActive = true },
                                new Product { Name = "Smartwatch", Description = "Fitness tracker and notifications", Price = 120m, Stock = 25, CategoryId = electronicsId, StoreId = 3, IsActive = true },

                                // Clothing
                                new Product { Name = "Men's T-Shirt", Description = "100% cotton t-shirt", Price = 25m, Stock = 50, CategoryId = clothingId, StoreId = 3, IsActive = true },
                                new Product { Name = "Women's Jeans", Description = "Slim fit denim jeans", Price = 45m, Stock = 40, CategoryId = clothingId, StoreId = 3, IsActive = true },
                                new Product { Name = "Unisex Hoodie", Description = "Comfortable hoodie for all seasons", Price = 35m, Stock = 30, CategoryId = clothingId, StoreId = 3, IsActive = true },

                                // Home & Kitchen
                                new Product { Name = "Blender", Description = "High-speed kitchen blender", Price = 60m, Stock = 20, CategoryId = homeId, StoreId = 3, IsActive = true },
                                new Product { Name = "Coffee Maker", Description = "Automatic drip coffee maker", Price = 80m, Stock = 15, CategoryId = homeId, StoreId = 3, IsActive = true },
                                new Product { Name = "Vacuum Cleaner", Description = "Compact and powerful vacuum cleaner", Price = 120m, Stock = 10, CategoryId = homeId, StoreId = 3, IsActive = true }
                            };

                if (!context.Products.Any())
                {
                 

                    context.Products.AddRange(products);
                    context.SaveChanges();
                }

                Console.WriteLine($"Products after insert: {context.Products.Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB seeding failed: {ex.Message}");
            }

            Console.WriteLine("========== DB SEED END ==========");
        }
    }
}
