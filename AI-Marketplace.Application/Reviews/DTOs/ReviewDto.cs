using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Reviews.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;

        public int? ProductId { get; set; }
        public int? StoreId { get; set; }

        public int Rating { get; set; } 
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
