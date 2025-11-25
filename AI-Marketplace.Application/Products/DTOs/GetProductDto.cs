using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.DTOs
{
    public class GetProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();

    }
}
