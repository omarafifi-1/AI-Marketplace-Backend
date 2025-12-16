using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.DTOs
{
    public class VendorStatisticsDto
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalDeliveredOrders { get; set; }
        public int TotalOffers { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
