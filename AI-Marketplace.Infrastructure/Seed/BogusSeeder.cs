using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using AI_Marketplace.Infrastructure.Data;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Seed
{
    public class BogusSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly Random _random = new Random(12345); // Fixed seed for reproducibility

        private List<Category> _categories = new List<Category>();
        private List<ApplicationUser> _customers = new List<ApplicationUser>();
        private List<ApplicationUser> _sellers = new List<ApplicationUser>();
        private List<Store> _stores = new List<Store>();
        private List<Product> _products = new List<Product>();
        private List<Address> _addresses = new List<Address>();

        public BogusSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            Console.WriteLine("========== BOGUS SEEDING START ==========");

            // Ensure roles exist
            await EnsureRolesAsync();

            // Check if already seeded by looking at product images
            var existingProductImages = await _context.ProductImages.CountAsync();
            
            Console.WriteLine($"Current database state: {existingProductImages} product images");
            
            // Skip if we already have product images (more than 100 images means already seeded with new version)
            if (existingProductImages > 1000) // Changed from 100 to 1000
            {
                Console.WriteLine("Database already contains product images. Skipping...");
                return;
            }

            try
            {
                await LoadOrCreateCategoriesAsync();
                
                // Only seed users if they don't exist
                var existingCustomers = await _userManager.GetUsersInRoleAsync("Customer");
                var existingSellers = await _userManager.GetUsersInRoleAsync("Seller");
                
                if (existingCustomers.Count < 40)
                {
                    await SeedCustomersAsync();
                }
                else
                {
                    _customers = existingCustomers.ToList();
                    Console.WriteLine($"Using existing {_customers.Count} customers");
                }
                
                if (existingSellers.Count < 20)
                {
                    await SeedSellersAndStoresAsync();
                }
                else
                {
                    _sellers = existingSellers.ToList();
                    _stores = await _context.Stores.ToListAsync();
                    Console.WriteLine($"Using existing {_sellers.Count} sellers and {_stores.Count} stores");
                }
                
                // Delete old products and images, then reseed with new images
                await ReseedProductsWithImagesAsync();
                
                // Check if addresses exist
                var existingAddresses = await _context.Addresses.CountAsync();
                if (existingAddresses < 50)
                {
                    await SeedAddressesAsync();
                }
                else
                {
                    _addresses = await _context.Addresses.ToListAsync();
                    Console.WriteLine($"Using existing {_addresses.Count} addresses");
                }
                
                await SeedCustomRequestsAsync();
                await SeedOrdersAsync();
                await SeedOffersAsync();

                Console.WriteLine("========== BOGUS SEEDING COMPLETED ==========");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task EnsureRolesAsync()
        {
            string[] roles = { "Admin", "Seller", "Customer" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
        }

        private async Task LoadOrCreateCategoriesAsync()
        {
            Console.WriteLine("Loading or creating Categories...");

            var requiredCategories = new[]
            {
                "Plushies",
                "Bracelets",
                "Mugs",
                "Home & Kitchen",
                "Accessories",
                "Clothing",
                "Electronics"
            };

            var categoryDescriptions = new Dictionary<string, string>
            {
                { "Plushies", "Soft, cuddly stuffed toys and collectibles" },
                { "Bracelets", "Handmade and designer bracelets for all occasions" },
                { "Mugs", "Custom and themed mugs for coffee and tea lovers" },
                { "Home & Kitchen", "Essential items and decor for your home" },
                { "Accessories", "Fashion accessories including bags, belts, and jewelry" },
                { "Clothing", "Trendy apparel and fashion wear" },
                { "Electronics", "Latest gadgets and electronic devices" }
            };

            // Load existing categories from database
            var existingCategories = await _context.Categories.ToListAsync();
            
            foreach (var name in requiredCategories)
            {
                var existing = existingCategories.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                
                if (existing != null)
                {
                    _categories.Add(existing);
                    Console.WriteLine($"Using existing category: {name}");
                }
                else
                {
                    var category = new Category
                    {
                        Name = name,
                        Description = categoryDescriptions[name],
                        CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 180))
                    };
                    _context.Categories.Add(category);
                    _categories.Add(category);
                    Console.WriteLine($"Creating new category: {name}");
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Total categories available: {_categories.Count}");
        }

        private async Task SeedCustomersAsync()
        {
            Console.WriteLine("Seeding Customers...");

            var customerFaker = new Faker<ApplicationUser>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.UserName, (f, u) => $"{u.FirstName.ToLower()}{u.LastName.ToLower()}{f.Random.Number(10, 999)}")
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.EmailConfirmed, f => true)
                .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow.AddDays(-f.Random.Number(1, 365)))
                .RuleFor(u => u.IsActive, f => true);

            for (int i = 0; i < 50; i++)
            {
                var customer = customerFaker.Generate();
                var result = await _userManager.CreateAsync(customer, "Customer@123");
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(customer, "Customer");
                    _customers.Add(customer);
                }
                else
                {
                    Console.WriteLine($"Failed to create customer: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            Console.WriteLine($"Created {_customers.Count} customers");
        }

        private async Task SeedSellersAndStoresAsync()
        {
            Console.WriteLine("Seeding Sellers and Stores...");

            // Store themes with their category mappings
            var storeThemes = new[]
            {
                new { Name = "Cuddle Critters", Categories = new[] { "Plushies" }, Description = "Premium handcrafted plush toys for all ages" },
                new { Name = "Plush Paradise", Categories = new[] { "Plushies" }, Description = "Your one-stop shop for adorable stuffed animals" },
                new { Name = "Soft Haven", Categories = new[] { "Plushies" }, Description = "Collectible and custom plush creations" },
                
                new { Name = "Charm Crafters", Categories = new[] { "Bracelets", "Accessories" }, Description = "Artisan jewelry and handmade bracelets" },
                new { Name = "Bead Boutique", Categories = new[] { "Bracelets", "Accessories" }, Description = "Designer bracelets with unique gemstones" },
                new { Name = "Wrist Wonders", Categories = new[] { "Bracelets" }, Description = "Custom personalized bracelet designs" },
                
                new { Name = "Mug Life Co", Categories = new[] { "Mugs", "Home & Kitchen" }, Description = "Creative and custom printed mugs" },
                new { Name = "Sip & Style", Categories = new[] { "Mugs" }, Description = "Premium ceramic mugs for every occasion" },
                new { Name = "Brew Masters", Categories = new[] { "Mugs", "Home & Kitchen" }, Description = "Themed mugs for coffee enthusiasts" },
                
                new { Name = "Home Harmony", Categories = new[] { "Home & Kitchen" }, Description = "Essential home decor and kitchen items" },
                new { Name = "Kitchen Kingdom", Categories = new[] { "Home & Kitchen" }, Description = "Modern kitchen gadgets and accessories" },
                new { Name = "Cozy Corner", Categories = new[] { "Home & Kitchen", "Accessories" }, Description = "Home comfort and lifestyle products" },
                new { Name = "Nest & Nest", Categories = new[] { "Home & Kitchen" }, Description = "Elegant home essentials" },
                
                new { Name = "Style Station", Categories = new[] { "Accessories", "Clothing" }, Description = "Trendy fashion accessories and more" },
                new { Name = "Accessory Avenue", Categories = new[] { "Accessories" }, Description = "Chic bags, belts, and fashion items" },
                new { Name = "Glam Gallery", Categories = new[] { "Accessories", "Clothing" }, Description = "Fashion-forward accessories for all" },
                new { Name = "Luxe Lifestyle", Categories = new[] { "Accessories" }, Description = "Premium fashion accessories" },
                
                new { Name = "Fashion Forward", Categories = new[] { "Clothing" }, Description = "Contemporary clothing for modern style" },
                new { Name = "Urban Threads", Categories = new[] { "Clothing" }, Description = "Street style and casual wear" },
                new { Name = "Chic Boutique", Categories = new[] { "Clothing", "Accessories" }, Description = "Designer clothing and accessories" },
                new { Name = "Wardrobe Essentials", Categories = new[] { "Clothing" }, Description = "Quality basics and everyday wear" },
                new { Name = "Trendy Threads", Categories = new[] { "Clothing" }, Description = "Latest fashion trends and styles" },
                
                new { Name = "TechVision Pro", Categories = new[] { "Electronics" }, Description = "Cutting-edge electronics and gadgets" },
                new { Name = "Gadget Galaxy", Categories = new[] { "Electronics" }, Description = "Latest tech devices and accessories" },
                new { Name = "Digital Dreams", Categories = new[] { "Electronics" }, Description = "Smart home and personal electronics" },
                new { Name = "ElectroHub", Categories = new[] { "Electronics" }, Description = "Premium electronic devices" },
                new { Name = "Circuit City", Categories = new[] { "Electronics" }, Description = "All your electronic needs in one place" },
                
                new { Name = "Mixed Market", Categories = new[] { "Clothing", "Accessories", "Home & Kitchen" }, Description = "Variety store with fashion and home items" },
                new { Name = "Everything Store", Categories = new[] { "Accessories", "Mugs", "Plushies" }, Description = "Gifts and collectibles for everyone" },
                new { Name = "General Goods", Categories = new[] { "Home & Kitchen", "Accessories", "Clothing" }, Description = "General merchandise and lifestyle products" }
            };

            var sellerFaker = new Faker<ApplicationUser>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.UserName, (f, u) => $"{u.FirstName.ToLower()}{u.LastName.ToLower()}_seller{f.Random.Number(10, 999)}")
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.EmailConfirmed, f => true)
                .RuleFor(u => u.CreatedAt, f => DateTime.UtcNow.AddDays(-f.Random.Number(30, 720)))
                .RuleFor(u => u.IsActive, f => true);

            for (int i = 0; i < 30 && i < storeThemes.Length; i++)
            {
                var seller = sellerFaker.Generate();
                var result = await _userManager.CreateAsync(seller, "Seller@123");
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(seller, "Seller");
                    _sellers.Add(seller);

                    var theme = storeThemes[i];
                    var store = new Store
                    {
                        OwnerId = seller.Id,
                        StoreName = theme.Name,
                        Description = theme.Description,
                        ContactEmail = seller.Email,
                        ContactPhone = new Faker().Phone.PhoneNumber(),
                        IsActive = true,
                        IsVerified = _random.Next(0, 10) > 2, // 80% verified
                        Rating = (decimal)(_random.NextDouble() * 2 + 3), // 3.0 - 5.0
                        TotalReviews = _random.Next(5, 200),
                        CreatedAt = seller.CreatedAt.AddDays(_random.Next(1, 10)),
                        LogoUrl = $"https://placehold.co/200x200/png?text={Uri.EscapeDataString(theme.Name)}",
                        BannerUrl = $"https://placehold.co/1200x300/png?text={Uri.EscapeDataString(theme.Name)}"
                    };

                    _stores.Add(store);
                }
                else
                {
                    Console.WriteLine($"Failed to create seller: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            _context.Stores.AddRange(_stores);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Created {_sellers.Count} sellers and {_stores.Count} stores");
        }

        private async Task SeedProductsAsync()
        {
            Console.WriteLine("Seeding Products...");

            var productTemplates = new Dictionary<string, List<(string Name, string ImageQuery)>>
            {
                { "Plushies", new List<(string, string)> { 
                    ("Teddy Bear", "teddy-bear"), 
                    ("Unicorn Plush", "unicorn-plush"), 
                    ("Dragon Plush", "dragon-toy"), 
                    ("Bunny Rabbit", "rabbit-plush"), 
                    ("Cat Plush", "cat-stuffed-animal"), 
                    ("Dog Plush", "dog-plush"), 
                    ("Penguin Plush", "penguin-toy"), 
                    ("Elephant Plush", "elephant-plush"), 
                    ("Fox Plush", "fox-stuffed-animal"), 
                    ("Panda Plush", "panda-plush") 
                }},
                { "Bracelets", new List<(string, string)> { 
                    ("Silver Chain Bracelet", "silver-bracelet"), 
                    ("Leather Wrap Bracelet", "leather-bracelet"), 
                    ("Beaded Bracelet", "beaded-bracelet"), 
                    ("Charm Bracelet", "charm-bracelet"), 
                    ("Gold Bangle", "gold-bangle"), 
                    ("Friendship Bracelet", "friendship-bracelet"), 
                    ("Crystal Bracelet", "crystal-bracelet"), 
                    ("Pearl Bracelet", "pearl-bracelet"), 
                    ("Rope Bracelet", "rope-bracelet"), 
                    ("Cuff Bracelet", "cuff-bracelet") 
                }},
                { "Mugs", new List<(string, string)> { 
                    ("Coffee Mug", "coffee-mug"), 
                    ("Travel Mug", "travel-mug"), 
                    ("Ceramic Mug", "ceramic-mug"), 
                    ("Glass Mug", "glass-mug"), 
                    ("Stainless Steel Mug", "steel-mug"), 
                    ("Color Changing Mug", "color-changing-mug"), 
                    ("Personalized Mug", "custom-mug"), 
                    ("Funny Quote Mug", "funny-mug"), 
                    ("Holiday Mug", "holiday-mug"), 
                    ("Tea Mug Set", "tea-mug") 
                }},
                { "Home & Kitchen", new List<(string, string)> { 
                    ("Kitchen Knife Set", "knife-set"), 
                    ("Cutting Board", "cutting-board"), 
                    ("Storage Container Set", "storage-containers"), 
                    ("Spice Rack", "spice-rack"), 
                    ("Utensil Holder", "utensil-holder"), 
                    ("Dish Rack", "dish-rack"), 
                    ("Mixing Bowls", "mixing-bowls"), 
                    ("Wall Clock", "wall-clock"), 
                    ("Decorative Pillow", "throw-pillow"), 
                    ("Table Runner", "table-runner") 
                }},
                { "Accessories", new List<(string, string)> { 
                    ("Leather Wallet", "leather-wallet"), 
                    ("Crossbody Bag", "crossbody-bag"), 
                    ("Sunglasses", "sunglasses"), 
                    ("Watch", "wristwatch"), 
                    ("Scarf", "scarf"), 
                    ("Hat", "fashion-hat"), 
                    ("Belt", "leather-belt"), 
                    ("Keychain", "keychain"), 
                    ("Phone Case", "phone-case"), 
                    ("Handbag", "handbag") 
                }},
                { "Clothing", new List<(string, string)> { 
                    ("T-Shirt", "tshirt"), 
                    ("Hoodie", "hoodie"), 
                    ("Jeans", "denim-jeans"), 
                    ("Jacket", "jacket"), 
                    ("Dress", "dress"), 
                    ("Sweater", "sweater"), 
                    ("Shorts", "shorts"), 
                    ("Leggings", "leggings"), 
                    ("Button-Up Shirt", "button-shirt"), 
                    ("Polo Shirt", "polo-shirt") 
                }},
                { "Electronics", new List<(string, string)> { 
                    ("Wireless Earbuds", "wireless-earbuds"), 
                    ("Smart Watch", "smartwatch"), 
                    ("Portable Charger", "power-bank"), 
                    ("Bluetooth Speaker", "bluetooth-speaker"), 
                    ("USB Cable", "usb-cable"), 
                    ("Phone Stand", "phone-stand"), 
                    ("Laptop Sleeve", "laptop-sleeve"), 
                    ("Webcam", "webcam"), 
                    ("Mouse Pad", "mousepad"), 
                    ("LED Desk Lamp", "desk-lamp") 
                }}
            };

            var storeThemeCategories = new[]
            {
                new { Name = "Cuddle Critters", Categories = new[] { "Plushies" } },
                new { Name = "Plush Paradise", Categories = new[] { "Plushies" } },
                new { Name = "Soft Haven", Categories = new[] { "Plushies" } },
                new { Name = "Charm Crafters", Categories = new[] { "Bracelets", "Accessories" } },
                new { Name = "Bead Boutique", Categories = new[] { "Bracelets", "Accessories" } },
                new { Name = "Wrist Wonders", Categories = new[] { "Bracelets" } },
                new { Name = "Mug Life Co", Categories = new[] { "Mugs", "Home & Kitchen" } },
                new { Name = "Sip & Style", Categories = new[] { "Mugs" } },
                new { Name = "Brew Masters", Categories = new[] { "Mugs", "Home & Kitchen" } },
                new { Name = "Home Harmony", Categories = new[] { "Home & Kitchen" } },
                new { Name = "Kitchen Kingdom", Categories = new[] { "Home & Kitchen" } },
                new { Name = "Cozy Corner", Categories = new[] { "Home & Kitchen", "Accessories" } },
                new { Name = "Nest & Nest", Categories = new[] { "Home & Kitchen" } },
                new { Name = "Style Station", Categories = new[] { "Accessories", "Clothing" } },
                new { Name = "Accessory Avenue", Categories = new[] { "Accessories" } },
                new { Name = "Glam Gallery", Categories = new[] { "Accessories", "Clothing" } },
                new { Name = "Luxe Lifestyle", Categories = new[] { "Accessories" } },
                new { Name = "Fashion Forward", Categories = new[] { "Clothing" } },
                new { Name = "Urban Threads", Categories = new[] { "Clothing" } },
                new { Name = "Chic Boutique", Categories = new[] { "Clothing", "Accessories" } },
                new { Name = "Wardrobe Essentials", Categories = new[] { "Clothing" } },
                new { Name = "Trendy Threads", Categories = new[] { "Clothing" } },
                new { Name = "TechVision Pro", Categories = new[] { "Electronics" } },
                new { Name = "Gadget Galaxy", Categories = new[] { "Electronics" } },
                new { Name = "Digital Dreams", Categories = new[] { "Electronics" } },
                new { Name = "ElectroHub", Categories = new[] { "Electronics" } },
                new { Name = "Circuit City", Categories = new[] { "Electronics" } },
                new { Name = "Mixed Market", Categories = new[] { "Clothing", "Accessories", "Home & Kitchen" } },
                new { Name = "Everything Store", Categories = new[] { "Accessories", "Mugs", "Plushies" } },
                new { Name = "General Goods", Categories = new[] { "Home & Kitchen", "Accessories", "Clothing" } }
            };

            var productImages = new List<ProductImage>();
            int imageId = 100; // Starting seed for varied images

            foreach (var store in _stores)
            {
                var storeTheme = storeThemeCategories.FirstOrDefault(st => st.Name == store.StoreName);
                if (storeTheme == null) continue;

                var productsToCreate = 10;
                var createdCount = 0;

                foreach (var categoryName in storeTheme.Categories)
                {
                    var category = _categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                    if (category == null || !productTemplates.ContainsKey(categoryName)) continue;

                    var templates = productTemplates[categoryName];
                    var productsPerCategory = Math.Min(productsToCreate - createdCount, templates.Count);
                    
                    var shuffledTemplates = templates.OrderBy(x => _random.Next()).Take(productsPerCategory).ToList();

                    foreach (var (name, imageQuery) in shuffledTemplates)
                    {
                        if (createdCount >= productsToCreate) break;

                        var faker = new Faker();
                        var product = new Product
                        {
                            StoreId = store.Id,
                            Name = name,
                            Description = faker.Commerce.ProductDescription(),
                            Price = decimal.Parse(faker.Commerce.Price(5, 500, 2)),
                            Stock = _random.Next(0, 200),
                            CategoryId = category.Id,
                            IsActive = true,
                            Rating = (decimal)(_random.NextDouble() * 2 + 3),
                            TotalReviews = _random.Next(0, 150),
                            CreatedAt = store.CreatedAt.AddDays(_random.Next(1, 30))
                        };

                        _products.Add(product);

                        // Create product images with better URLs
                        productImages.Add(new ProductImage
                        {
                            Product = product,
                            ImageUrl = $"https://picsum.photos/seed/{imageQuery}-{imageId}/600/600",
                            IsPrimary = true,
                            UploadedAt = product.CreatedAt
                        });

                        // Add 1-2 additional images per product
                        var additionalImages = _random.Next(1, 3);
                        for (int i = 1; i <= additionalImages; i++)
                        {
                            productImages.Add(new ProductImage
                            {
                                Product = product,
                                ImageUrl = $"https://picsum.photos/seed/{imageQuery}-{imageId + i}/600/600",
                                IsPrimary = false,
                                UploadedAt = product.CreatedAt
                            });
                        }

                        imageId += 10; // Increment to get different images
                        createdCount++;
                    }
                }

                // Fill remaining slots if needed
                while (createdCount < productsToCreate && storeTheme.Categories.Length > 0)
                {
                    var randomCategoryName = storeTheme.Categories[_random.Next(storeTheme.Categories.Length)];
                    var category = _categories.FirstOrDefault(c => c.Name.Equals(randomCategoryName, StringComparison.OrdinalIgnoreCase));
                    
                    if (category != null && productTemplates.ContainsKey(randomCategoryName))
                    {
                        var templates = productTemplates[randomCategoryName];
                        var (name, imageQuery) = templates[_random.Next(templates.Count)];
                        
                        var faker = new Faker();
                        var product = new Product
                        {
                            StoreId = store.Id,
                            Name = $"{name} ({_random.Next(100, 999)})",
                            Description = faker.Commerce.ProductDescription(),
                            Price = decimal.Parse(faker.Commerce.Price(5, 500, 2)),
                            Stock = _random.Next(0, 200),
                            CategoryId = category.Id,
                            IsActive = true,
                            Rating = (decimal)(_random.NextDouble() * 2 + 3),
                            TotalReviews = _random.Next(0, 150),
                            CreatedAt = store.CreatedAt.AddDays(_random.Next(1, 30))
                        };

                        _products.Add(product);

                        // Add images for this product
                        productImages.Add(new ProductImage
                        {
                            Product = product,
                            ImageUrl = $"https://picsum.photos/seed/{imageQuery}-{imageId}/600/600",
                            IsPrimary = true,
                            UploadedAt = product.CreatedAt
                        });

                        var additionalImages = _random.Next(1, 3);
                        for (int i = 1; i <= additionalImages; i++)
                        {
                            productImages.Add(new ProductImage
                            {
                                Product = product,
                                ImageUrl = $"https://picsum.photos/seed/{imageQuery}-{imageId + i}/600/600",
                                IsPrimary = false,
                                UploadedAt = product.CreatedAt
                            });
                        }

                        imageId += 10;
                        createdCount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _context.Products.AddRange(_products);
            await _context.SaveChangesAsync();

            _context.ProductImages.AddRange(productImages);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Created {_products.Count} products with {productImages.Count} images");
        }

        private async Task SeedAddressesAsync()
        {
            Console.WriteLine("Seeding Addresses...");

            var addressFaker = new Faker<Address>()
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.State, f => f.Address.State())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.Country, f => f.Address.Country())
                .RuleFor(a => a.IsPrimary, f => true)
                .RuleFor(a => a.CreatedAt, f => DateTime.UtcNow.AddDays(-f.Random.Number(1, 180)));

            foreach (var customer in _customers)
            {
                var addressCount = _random.Next(1, 3);
                for (int i = 0; i < addressCount; i++)
                {
                    var address = addressFaker.Generate();
                    address.UserId = customer.Id;
                    address.IsPrimary = i == 0;
                    _addresses.Add(address);
                }
            }

            _context.Addresses.AddRange(_addresses);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Created {_addresses.Count} addresses");
        }

        private async Task SeedCustomRequestsAsync()
        {
            Console.WriteLine("Seeding Custom Requests...");

            var requestDescriptions = new Dictionary<string, List<string>>
            {
                { "Plushies", new List<string> { 
                    "Looking for a custom giant teddy bear for anniversary gift",
                    "Need a personalized plush toy of my pet cat",
                    "Want a handmade unicorn plush with rainbow colors",
                    "Looking for a set of mini animal plushies for decoration"
                }},
                { "Bracelets", new List<string> { 
                    "Need a custom engraved bracelet with my partner's name",
                    "Looking for a birthstone bracelet with specific gemstones",
                    "Want a leather bracelet with custom stamping",
                    "Need matching friendship bracelets for my group"
                }},
                { "Mugs", new List<string> { 
                    "Looking for custom photo mugs for wedding favors",
                    "Need a mug with my company logo for corporate gifts",
                    "Want a personalized mug with custom artwork",
                    "Looking for heat-sensitive color changing mugs with custom design"
                }},
                { "Home & Kitchen", new List<string> { 
                    "Need custom kitchen organizers for small space",
                    "Looking for personalized cutting board with engraving",
                    "Want custom embroidered throw pillows for living room",
                    "Need unique wall art for my kitchen"
                }},
                { "Accessories", new List<string> { 
                    "Looking for a custom leather wallet with monogram",
                    "Need a personalized phone case with my design",
                    "Want a custom tote bag for daily use",
                    "Looking for a unique handmade scarf"
                }},
                { "Clothing", new List<string> { 
                    "Need custom t-shirts for my team event",
                    "Looking for a personalized hoodie with custom print",
                    "Want a custom denim jacket with patches",
                    "Need embroidered polo shirts for business"
                }},
                { "Electronics", new List<string> { 
                    "Looking for a custom gaming setup accessories",
                    "Need personalized tech organizer for cables",
                    "Want a custom laptop skin with my artwork",
                    "Looking for branded USB drives for my business"
                }}
            };

            var customRequests = new List<CustomRequest>();

            foreach (var customer in _customers)
            {
                var requestCount = _random.Next(1, 6);
                
                for (int i = 0; i < requestCount; i++)
                {
                    var category = _categories[_random.Next(_categories.Count)];
                    var descriptions = requestDescriptions.ContainsKey(category.Name) 
                        ? requestDescriptions[category.Name] 
                        : new List<string> { "Custom request for special item" };
                    
                    var description = descriptions[_random.Next(descriptions.Count)];
                    
                    var customRequest = new CustomRequest
                    {
                        BuyerId = customer.Id,
                        Description = description,
                        CategoryId = category.Id,
                        Status = CustomRequestStatus.Open,
                        Budget = _random.Next(50, 500),
                        Deadline = DateTime.UtcNow.AddDays(_random.Next(7, 60)),
                        CreatedAt = customer.CreatedAt.AddDays(_random.Next(1, 30))
                    };

                    customRequests.Add(customRequest);
                }
            }

            _context.CustomRequests.AddRange(customRequests);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Created {customRequests.Count} custom requests");
        }

        private async Task SeedOrdersAsync()
        {
            Console.WriteLine("Seeding Orders and Order Items...");

            var masterOrders = new List<MasterOrder>();
            var orders = new List<Order>();
            var orderItems = new List<OrderItem>();

            foreach (var customer in _customers)
            {
                var orderCount = _random.Next(2, 11);
                var customerAddresses = _addresses.Where(a => a.UserId == customer.Id).ToList();
                
                if (!customerAddresses.Any()) continue;

                for (int i = 0; i < orderCount; i++)
                {
                    var shippingAddress = customerAddresses[_random.Next(customerAddresses.Count)];
                    
                    // Create master order
                    var masterOrder = new MasterOrder
                    {
                        BuyerId = customer.Id,
                        TotalAmount = 0,
                        Status = GetRandomOrderStatus(),
                        ShippingAddressId = shippingAddress.Id,
                        CreatedAt = customer.CreatedAt.AddDays(_random.Next(5, 60))
                    };
                    masterOrders.Add(masterOrder);
                }
            }

            _context.MasterOrders.AddRange(masterOrders);
            await _context.SaveChangesAsync();

            // Create child orders for each master order
            foreach (var masterOrder in masterOrders)
            {
                var storeCount = _random.Next(1, 4);
                var selectedStores = _stores.OrderBy(x => _random.Next()).Take(storeCount).ToList();

                foreach (var store in selectedStores)
                {
                    var order = new Order
                    {
                        BuyerId = masterOrder.BuyerId,
                        StoreId = store.Id,
                        MasterOrderId = masterOrder.Id,
                        TotalAmount = 0,
                        Status = masterOrder.Status,
                        ShippingAddressId = masterOrder.ShippingAddressId,
                        OrderDate = masterOrder.CreatedAt
                    };

                    if (masterOrder.Status == "Delivered")
                    {
                        order.DeliveredAt = masterOrder.CreatedAt.AddDays(_random.Next(3, 14));
                    }

                    orders.Add(order);
                }
            }

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Create order items
            foreach (var order in orders)
            {
                var itemCount = _random.Next(1, 6);
                var storeProducts = _products.Where(p => p.StoreId == order.StoreId && p.Stock > 0).ToList();
                
                if (!storeProducts.Any()) continue;

                var selectedProducts = storeProducts.OrderBy(x => _random.Next()).Take(itemCount).ToList();
                decimal orderTotal = 0;

                foreach (var product in selectedProducts)
                {
                    var quantity = _random.Next(1, 4);
                    var unitPrice = product.Price;
                    var totalPrice = unitPrice * quantity;

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = totalPrice
                    };

                    orderItems.Add(orderItem);
                    orderTotal += totalPrice;
                }

                order.TotalAmount = orderTotal;
            }

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            // Update master order totals
            foreach (var masterOrder in masterOrders)
            {
                masterOrder.TotalAmount = orders.Where(o => o.MasterOrderId == masterOrder.Id).Sum(o => o.TotalAmount);
                if (masterOrder.Status == "Delivered")
                {
                    masterOrder.CompletedAt = masterOrder.CreatedAt.AddDays(_random.Next(3, 14));
                }
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"Created {masterOrders.Count} master orders, {orders.Count} orders, and {orderItems.Count} order items");
        }

        private async Task SeedOffersAsync()
        {
            Console.WriteLine("Seeding Offers...");

            var customRequests = await _context.CustomRequests
                .Where(cr => cr.Status == CustomRequestStatus.Open)
                .ToListAsync();

            var offers = new List<Offer>();

            foreach (var store in _stores)
            {
                var offerCount = _random.Next(2, 6);
                var selectedRequests = customRequests.OrderBy(x => _random.Next()).Take(offerCount).ToList();

                foreach (var request in selectedRequests)
                {
                    // Check if this store already has an offer for this request
                    if (offers.Any(o => o.StoreId == store.Id && o.CustomRequestId == request.Id))
                        continue;

                    var budgetMultiplier = 0.7m + ((decimal)_random.NextDouble() * 0.6m); // 0.7 - 1.3
                    var proposedPrice = request.Budget.HasValue 
                        ? request.Budget.Value * budgetMultiplier 
                        : _random.Next(100, 600);

                    var offer = new Offer
                    {
                        CustomRequestId = request.Id,
                        StoreId = store.Id,
                        ProposedPrice = Math.Round(proposedPrice, 2),
                        EstimatedDays = _random.Next(3, 30),
                        Message = $"We can fulfill your request with high quality. Estimated delivery in {_random.Next(3, 30)} days.",
                        Status = "Pending",
                        CreatedAt = request.CreatedAt.AddDays(_random.Next(1, 5))
                    };

                    offers.Add(offer);
                }
            }

            _context.Offers.AddRange(offers);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Created {offers.Count} offers");
        }

        private string GetRandomOrderStatus()
        {
            var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Delivered", "Delivered" }; // Weight toward delivered
            return statuses[_random.Next(statuses.Length)];
        }

        private async Task ReseedProductsWithImagesAsync()
        {
            Console.WriteLine("=========== REMOVING OLD PRODUCTS ===========");
            
            // CRITICAL: Delete in correct order to avoid FK violations
            Console.WriteLine("Deleting old order items...");
            var oldOrderItems = await _context.OrderItems.ToListAsync();
            _context.OrderItems.RemoveRange(oldOrderItems);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ Deleted {oldOrderItems.Count} order items");
            
            Console.WriteLine("Deleting old offers...");
            var oldOffers = await _context.Offers.ToListAsync();
            _context.Offers.RemoveRange(oldOffers);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ Deleted {oldOffers.Count} offers");
            
            Console.WriteLine("Deleting old orders...");
            var oldOrders = await _context.Orders.ToListAsync();
            _context.Orders.RemoveRange(oldOrders);
            await _context.SaveChangesAsync();
            Console.WriteLine($" Deleted {oldOrders.Count} orders");
            
            Console.WriteLine("Deleting old master orders...");
            var oldMasterOrders = await _context.MasterOrders.ToListAsync();
            _context.MasterOrders.RemoveRange(oldMasterOrders);
            await _context.SaveChangesAsync();
            Console.WriteLine($" Deleted {oldMasterOrders.Count} master orders");
            
            Console.WriteLine("Deleting old custom requests...");
            var oldCustomRequests = await _context.CustomRequests.ToListAsync();
            _context.CustomRequests.RemoveRange(oldCustomRequests);
            await _context.SaveChangesAsync();
            Console.WriteLine($" Deleted {oldCustomRequests.Count} custom requests");
            
            Console.WriteLine("Deleting old product images...");
            var oldImages = await _context.ProductImages.ToListAsync();
            _context.ProductImages.RemoveRange(oldImages);
            await _context.SaveChangesAsync();
            Console.WriteLine($" Deleted {oldImages.Count} product images");
            
            Console.WriteLine("Deleting old products...");
            var oldProducts = await _context.Products.ToListAsync();
            _context.Products.RemoveRange(oldProducts);
            await _context.SaveChangesAsync();
            Console.WriteLine($" Deleted {oldProducts.Count} products");
            
            Console.WriteLine("=========== CREATING NEW PRODUCTS WITH PICSUM IMAGES ===========");
            await SeedProductsAsync();
        }
    }
}