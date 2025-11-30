using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Vendors.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Queries
{
    public class GetVendorStatisticsQueryHandler : IRequestHandler<GetVendorStatisticsQuery, VendorStatisticsDto>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IProductRepository _productRepository;

        public GetVendorStatisticsQueryHandler(IStoreRepository storeRepository, IOrderRepository orderRepository, IOfferRepository offerRepository
            ,IProductRepository productRepository) 
        {
            _storeRepository = storeRepository;
            _orderRepository = orderRepository;
            _offerRepository = offerRepository;
            _productRepository = productRepository;
        }
        public async Task<VendorStatisticsDto> Handle(GetVendorStatisticsQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(request.VendorId, cancellationToken);
            if (store == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store not found." } }
                });
            }
            var totalProducts = await _productRepository.GetByStoreIdAsync(store.Id, cancellationToken);
            int totalProductsCount = totalProducts.Count();
            var totalOrders = await _orderRepository.GetOrdersByStoreIdAsync(store.Id, cancellationToken);
            int totalOrdersCount = totalOrders.Count();
            int totalDeliveredOrdersCount = totalOrders.Where(o => o.Status == "Delivered").Count();
            var totalOffers = await _offerRepository.GetByStoreIdAsync(store.Id, 1, 100000, cancellationToken);
            int totalOffersCount = totalOffers.TotalCount;
            decimal totalRevenue = totalOrders.Where(o => o.Status == "Delivered").Sum(o => o.TotalAmount);
            var statisticsDto = new VendorStatisticsDto
            {
                TotalProducts = totalProductsCount,
                TotalOrders = totalOrdersCount,
                TotalDeliveredOrders = totalDeliveredOrdersCount,
                TotalOffers = totalOffersCount,
                TotalRevenue = totalRevenue
            };
            return statisticsDto;
        }
    }
}
