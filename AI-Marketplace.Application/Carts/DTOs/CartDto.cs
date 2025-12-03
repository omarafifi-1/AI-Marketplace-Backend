using System;
using System.Collections.Generic;
using System.Linq;

namespace AI_Marketplace.Application.Carts.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public decimal CartTotalAmount => CartItems.Sum(item => item.TotalPrice);
        public int CartTotalItemsCount => CartItems.Sum(item => item.Quantity);
    }
}
