using AI_Marketplace.Application.Admin.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Queries
{
    public class GetPlatformStatsQueryHandler : IRequestHandler<GetPlatformStatsQuery, StatsDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetPlatformStatsQueryHandler(IProductRepository productRepository, IStoreRepository storeRepository, IOrderRepository orderRepository, UserManager<ApplicationUser> userManager, IMapper mapper) 
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<StatsDto> Handle(GetPlatformStatsQuery request, CancellationToken cancellationToken)
        {
            var totalUsers = _userManager.Users.Count();
            var totalProducts = _productRepository.GetQueryable().ToList().Count;
            var Orders = await _orderRepository.GetAllOrdersAsync(cancellationToken);
            var totalOrders = Orders.Count;
            decimal totalRevenue = 0;
            foreach (var order in Orders)
            {
                if (order.Status == "Delivered")
                {
                    totalRevenue +=  order.TotalAmount;
                }
            }
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            var recentOrders = Orders.Where(o => o.OrderDate >= oneWeekAgo).ToList();
            decimal weeklyRevenue = 0;
            foreach (var order in recentOrders)
            {
                if (order.Status == "Delivered")
                {
                    weeklyRevenue += order.TotalAmount;
                }
            }
            var weeklyNewUsers = _userManager.Users.Where(u => u.CreatedAt >= oneWeekAgo).ToList();
            List<UserResponseDto> mappedWeeklyNewUsers = new List<UserResponseDto>();
            foreach (var user in weeklyNewUsers)
            {
                mappedWeeklyNewUsers.Add(_mapper.Map<UserResponseDto>(user));
                mappedWeeklyNewUsers.Last().Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";
            }

            var statsDto = new StatsDto
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                WeeklyRevenue = weeklyRevenue,
                WeeklyNewUsers = mappedWeeklyNewUsers
            };

            return statsDto;
        }
    }
}
