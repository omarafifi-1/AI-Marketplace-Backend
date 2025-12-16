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

            // FILTERING

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword));
            }

            // TOTAL BEFORE PAGINATION
            var totalRecords = await query.CountAsync(cancellationToken);

            // SORTING
            query = sortBy switch
            {
                "price" => desc ? query.OrderByDescending(p => p.Price)
                                : query.OrderBy(p => p.Price),

                "name" => desc ? query.OrderByDescending(p => p.Name)
                                : query.OrderBy(p => p.Name),

                _ => desc ? query.OrderByDescending(p => p.CreatedAt)
                          : query.OrderBy(p => p.CreatedAt)
            };

            // INCLUDE IMAGES & STORE
            query = query
                .Include(p => p.ProductImages)
                .Include(p => p.Store);

            // PAGINATION
            var skip = (page - 1) * pageSize;

            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ProjectTo<GetProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // RETURN PAGED RESULT
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
