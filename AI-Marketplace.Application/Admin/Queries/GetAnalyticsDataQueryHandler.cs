using AI_Marketplace.Application.Admin.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Application.Vendors.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Queries
{
    public class GetAnalyticsDataQueryHandler : IRequestHandler<GetAnalyticsDataQuery, AnalyticsDto>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetAnalyticsDataQueryHandler(IStoreRepository storeRepository, IOrderRepository orderRepository, UserManager<ApplicationUser> userManager, IProductRepository productRepository, IMapper mapper) 
        {
            _storeRepository = storeRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<AnalyticsDto> Handle(GetAnalyticsDataQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow;
            var week1 = DateTime.UtcNow.AddDays(-7);
            var week2 = DateTime.UtcNow.AddDays(-14);
            var week3 = DateTime.UtcNow.AddDays(-21);
            var week4 = DateTime.UtcNow.AddDays(-28);
            var orders = await _orderRepository.GetAllOrdersAsync(cancellationToken);
            var stores = await _storeRepository.GetAllStoresAsync(cancellationToken);
            var products = _productRepository.GetQueryable().ToList();
            var users = _userManager.Users.ToList();
            
            var revenueWeek1 = orders.Where(o => o.OrderDate >= week1 && o.OrderDate <= today).Sum(o => o.TotalAmount);
            var revenueWeek2 = orders.Where(o => o.OrderDate >= week2 && o.OrderDate < week1).Sum(o => o.TotalAmount);
            var revenueWeek3 = orders.Where(o => o.OrderDate >= week3 && o.OrderDate < week2).Sum(o => o.TotalAmount);
            var revenueWeek4 = orders.Where(o => o.OrderDate >= week4 && o.OrderDate < week3).Sum(o => o.TotalAmount);
            var weeklyRevenue = new decimal[] { revenueWeek4, revenueWeek3, revenueWeek2, revenueWeek1 };
            
            var topStores = orders
                .GroupBy(o => o.StoreId)
                .Select(g => new TopStoreDto
                {
                    Id = g.Key,
                    StoreName = stores.FirstOrDefault(s => s.Id == g.Key)?.StoreName ?? "Unknown",
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    TotalOrders = g.Count()
                })
                .OrderByDescending(s => s.TotalRevenue)
                .Take(5)
                .ToList();
            
            var topProducts = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopProductDto
                {
                    Id = g.Key,
                    Name = products.FirstOrDefault(p => p.Id == g.Key)?.Name ?? "Unknown",
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(5)
                .ToList();
            
            var topUsers = orders
                .GroupBy(o => o.BuyerId)
                .Select(g => new TopUserDto
                {
                    Id = g.Key,
                    UserName = users.FirstOrDefault(u => u.Id == g.Key)?.UserName ?? "Unknown",
                    Email = users.FirstOrDefault(u => u.Id == g.Key)?.Email ?? "Unknown",
                    TotalSpending = g.Sum(o => o.TotalAmount),
                    TotalOrders = g.Count()
                })
                .OrderByDescending(u => u.TotalSpending)
                .Take(5)
                .ToList();

            var analyticsDto = new AnalyticsDto
            {
                WeeklyTotalRevenueForLastMonth = weeklyRevenue,
                TopProductsBySales = topProducts,
                TopSpendingUsers = topUsers,
                TopStoresByRevenue = topStores
            };
            return analyticsDto;
        }
    }
}
