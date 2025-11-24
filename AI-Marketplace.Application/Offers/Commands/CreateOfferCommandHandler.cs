using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Offers.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, OfferResponseDto>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOfferCommandHandler> _logger;

        public CreateOfferCommandHandler(
            IOfferRepository offerRepository,
            IStoreRepository storeRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<CreateOfferCommandHandler> logger)
        {
            _offerRepository = offerRepository;
            _storeRepository = storeRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OfferResponseDto> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            ValidateCommand(request);

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
                    "User {UserId} attempted to create offer without Vendor/Seller role",
                    request.UserId);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Authorization", new[] { "Only vendors can create offers." } }
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
                    { "Store", new[] { "Vendor must have an active store to create offers." } }
                });
            }

            var offer = new Offer
            {
                CustomRequestId = request.CustomRequestId,
                StoreId = store.Id,
                ProposedPrice = request.ProposedPrice,
                EstimatedDays = request.EstimatedDays,
                Message = request.Message?.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var createdOffer = await _offerRepository.AddAsync(offer, cancellationToken);

                createdOffer.Store = store;

                _logger.LogInformation(
                    "Offer created successfully: OfferId={OfferId}, StoreId={StoreId}, CustomRequestId={CustomRequestId}",
                    createdOffer.Id,
                    createdOffer.StoreId,
                    createdOffer.CustomRequestId);

                return _mapper.Map<OfferResponseDto>(createdOffer);
            }
            catch (DuplicateOfferException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Duplicate offer attempt: StoreId={StoreId}, CustomRequestId={CustomRequestId}",
                    store.Id,
                    request.CustomRequestId);

                throw;
            }
        }

        private void ValidateCommand(CreateOfferCommand request)
        {
            var errors = new Dictionary<string, string[]>();

            if (request.CustomRequestId <= 0)
            {
                errors.Add("CustomRequestId", new[] { "CustomRequestId must be a positive integer." });
            }

            if (request.ProposedPrice <= 0)
            {
                errors.Add("ProposedPrice", new[] { "ProposedPrice must be greater than 0." });
            }
            else if (request.ProposedPrice > 999999.99m)
            {
                errors.Add("ProposedPrice", new[] { "ProposedPrice cannot exceed 999,999.99." });
            }

            if (request.EstimatedDays < 1)
            {
                errors.Add("EstimatedDays", new[] { "EstimatedDays must be at least 1." });
            }
            else if (request.EstimatedDays > 365)
            {
                errors.Add("EstimatedDays", new[] { "EstimatedDays cannot exceed 365." });
            }

            if (errors.Any())
            {
                _logger.LogWarning(
                    "Offer creation validation failed: {Errors}",
                    string.Join(", ", errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")));
                throw new ValidationException(errors);
            }
        }
    }
}