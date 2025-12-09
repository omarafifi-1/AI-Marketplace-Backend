using System;
using System.Collections.Generic;

namespace AI_Marketplace.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }

        // Optional owners
        public int? UserId { get; set; }
        public int? StoreId { get; set; }

        // Address fields
        public string Street { get; set; } = string.Empty;
        public string? SuiteOrUnit { get; set; }
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public Store? Store { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
