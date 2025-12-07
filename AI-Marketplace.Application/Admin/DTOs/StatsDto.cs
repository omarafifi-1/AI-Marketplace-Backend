using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.DTOs
{
    public class StatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public List<UserResponseDto>? WeeklyNewUsers { get; set; }
    }
}
