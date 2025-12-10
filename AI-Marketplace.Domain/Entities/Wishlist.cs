using System;
using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Domain.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        
        public DateTime AddedOn { get; set; } = DateTime.UtcNow;        
        public ApplicationUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
