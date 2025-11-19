using System;
using System.Collections.Generic;

namespace AI_Marketplace.Domain.Entities
{
    public class ChatSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}