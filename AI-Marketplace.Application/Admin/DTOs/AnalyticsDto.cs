using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Application.Vendors.DTOs;
using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.DTOs
{
    public class AnalyticsDto
    {
        public decimal[] WeeklyTotalRevenueForLastMonth { get; set; } = new decimal[4];
        public List<TopStoreDto> TopStoresByRevenue { get; set; } = new();
        public List<TopProductDto> TopProductsBySales { get; set; } = new();
        public List<TopUserDto> TopSpendingUsers { get; set; } = new();
    }
}
