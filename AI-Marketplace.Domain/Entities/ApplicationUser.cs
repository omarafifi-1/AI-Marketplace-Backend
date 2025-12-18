using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace AI_Marketplace.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Store? Store { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<CustomRequest> CustomRequests { get; set; } = new List<CustomRequest>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
