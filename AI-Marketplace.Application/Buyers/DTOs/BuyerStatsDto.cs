using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Buyers.DTOs
{
    public class BuyerStatsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
