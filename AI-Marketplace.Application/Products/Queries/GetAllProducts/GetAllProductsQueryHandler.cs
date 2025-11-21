using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;

        }
        public async Task<PagedProductDto> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(request.Page, 1);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var sortBy = (request.SortBy ?? "date").Trim().ToLowerInvariant();
            var sortDirection = (request.SortDirection ?? "desc").Trim().ToLowerInvariant();
            var desc = sortDirection == "desc";

            var query = _productRepository
                .GetQueryable()
                .AsNoTracking()
                .Where(p => p.IsActive);
              
            var totalRecords = await query.CountAsync(cancellationToken);

            if (sortBy == "price")
            {
                query = desc ? query.OrderByDescending(p => p.Price)
                             : query.OrderBy(p => p.Price);
            }
            else
            {
                query = desc ? query.OrderByDescending(p => p.CreatedAt)
                             : query.OrderBy(p => p.CreatedAt);
            }

            query = query
                .Include(p => p.ProductImages)
                .Include(p => p.Store);

            var skip = (page - 1) * pageSize;

            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ProjectTo<GetProductDto>(_mapper.ConfigurationProvider)
                //.Select(p => new GetProductDto
                //{
                //    Id = p.Id,
                //    Name = p.Name,
                //    Description = p.Description,
                //    Price = p.Price,
                //    Stock = p.Stock,
                //    CreatedAt = p.CreatedAt,
                //    VendorName = p.Store != null ? p.Store.StoreName : string.Empty,
                //    ImageUrls = p.ProductImages
                //        .OrderByDescending(img => img.IsPrimary)
                //        .Select(img => img.ImageUrl)
                //        .ToList()
                //})
                .ToListAsync(cancellationToken);

            return new PagedProductDto
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = items
            };
        }
    }
}
