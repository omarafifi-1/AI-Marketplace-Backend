using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Offers.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AI_Marketplace.Application.Offers.Queries.GetOffersByStoreId
{
    public class GetOffersByStoreIdQueryHandler : IRequestHandler<GetOffersByStoreIdQuery, PagedOfferDto>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOffersByStoreIdQueryHandler> _logger;

        public GetOffersByStoreIdQueryHandler(
            IOfferRepository offerRepository,
            IStoreRepository storeRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<GetOffersByStoreIdQueryHandler> logger)
        {
            _offerRepository = offerRepository;
            _storeRepository = storeRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedOfferDto> Handle(
            GetOffersByStoreIdQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User not found: UserId={UserId}", request.UserId);
                throw new UnauthorizedAccessException("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Vendor") && !roles.Contains("Seller"))
            {
                _logger.LogWarning(
                    "User {UserId} attempted to retrieve store offers without Vendor/Seller role",
                    request.UserId);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Authorization", new[] { "Only vendors can retrieve store offers." } }
                });
            }

            var store = await _storeRepository.GetByOwnerIdAsync(user.Id, cancellationToken);
            if (store == null || !store.IsActive)
            {
                _logger.LogWarning(
                    "User {UserId} has no active store",
                    request.UserId);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "You must have an active store to retrieve offers." } }
                });
            }

            var page = Math.Max(request.Page, 1);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            _logger.LogInformation(
                "Retrieving offers for StoreId={StoreId}, Page={Page}, PageSize={PageSize}",
                store.Id,
                page,
                pageSize);

            var (totalCount, offers) = await _offerRepository.GetByStoreIdAsync(
                store.Id,
                page,
                pageSize,
                cancellationToken);

            _logger.LogInformation(
                "Found {Count} offers for StoreId={StoreId} (TotalCount={TotalCount})",
                offers.Count,
                store.Id,
                totalCount);

            var offerDtos = _mapper.Map<List<OfferResponseDto>>(offers);

            return new PagedOfferDto
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalCount,
                Items = offerDtos
            };
        }
    }
}