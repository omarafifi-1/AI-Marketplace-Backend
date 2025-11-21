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
            

                // PRODUCTS
                Console.WriteLine($"Products count: {context.Products.Count()}");
                if (!context.Products.Any())
                {
                    context.Products.Add(new Product
                    {
                        Name = "Wireless Headphones",
                        Description = "Bluetooth noise-cancelling headphones",
                        Price = 150m,
                        Stock = 20,
                        CategoryId = electronicsId,
                        StoreId = 3,
                        IsActive = true
                    });
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
