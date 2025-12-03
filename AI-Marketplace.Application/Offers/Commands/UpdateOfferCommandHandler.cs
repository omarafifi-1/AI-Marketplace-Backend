using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Offers.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, OfferResponseDto>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOfferCommandHandler> _logger;

        public UpdateOfferCommandHandler(
            IOfferRepository offerRepository,
            IStoreRepository storeRepository,
            IMapper mapper,
            ILogger<UpdateOfferCommandHandler> logger)
        {
            _offerRepository = offerRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OfferResponseDto> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting offer update: OfferId={OfferId}, UserId={UserId}",
                request.OfferId,
                request.UserId);

            // 1. Validate command input
            ValidateCommand(request);

            // 2. Get authenticated user's store
            var store = await _storeRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);
            if (store == null || !store.IsActive)
            {
                _logger.LogWarning(
                    "User has no active store: UserId={UserId}",
                    request.UserId);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Vendor must have an active store to update offers." } }
                });
            }

            _logger.LogInformation(
                "Store retrieved: StoreId={StoreId}, StoreName={StoreName}",
                store.Id,
                store.StoreName);

            // 3. Retrieve offer by ID
            var offer = await _offerRepository.GetByIdAsync(request.OfferId, cancellationToken);
            if (offer == null)
            {
                _logger.LogWarning("Offer not found: OfferId={OfferId}", request.OfferId);
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Offer", new[] { $"Offer with ID {request.OfferId} not found." } }
                });
            }

            _logger.LogInformation(
                "Offer retrieved: OfferId={OfferId}, Status={Status}, StoreId={StoreId}",
                offer.Id,
                offer.Status,
                offer.StoreId);

            // 4. Verify offer belongs to vendor's store
            if (offer.StoreId != store.Id)
            {
                _logger.LogWarning(
                    "Unauthorized offer update attempt: OfferId={OfferId}, OfferStoreId={OfferStoreId}, VendorStoreId={VendorStoreId}",
                    offer.Id,
                    offer.StoreId,
                    store.Id);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Authorization", new[] { "You can only update offers from your own store." } }
                });
            }

            // 5. Verify offer status is "Pending"
            if (offer.Status != "Pending")
            {
                _logger.LogWarning(
                    "Cannot update non-pending offer: OfferId={OfferId}, Status={Status}",
                    offer.Id,
                    offer.Status);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Offer", new[] { $"Only pending offers can be edited. Current status: {offer.Status}" } }
                });
            }

            // 6. Update offer properties
            var oldPrice = offer.ProposedPrice;
            var oldDays = offer.EstimatedDays;
            var oldMessage = offer.Message;

            offer.ProposedPrice = request.ProposedPrice;
            offer.EstimatedDays = request.EstimatedDays;
            offer.Message = request.Message?.Trim();

            _logger.LogInformation(
                "Updating offer: OfferId={OfferId}, Price: {OldPrice}?{NewPrice}, Days: {OldDays}?{NewDays}",
                offer.Id,
                oldPrice,
                offer.ProposedPrice,
                oldDays,
                offer.EstimatedDays);

            // 7. Save changes to repository
            try
            {
                await _offerRepository.UpdateAsync(offer, cancellationToken);

                _logger.LogInformation(
                    "Offer updated successfully: OfferId={OfferId}, StoreId={StoreId}",
                    offer.Id,
                    offer.StoreId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update offer: OfferId={OfferId}",
                    offer.Id);
                throw;
            }

            // 8. Retrieve updated offer with navigation properties for mapping
            var updatedOffer = await _offerRepository.GetByIdAsync(offer.Id, cancellationToken);
            if (updatedOffer == null)
            {
                _logger.LogError(
                    "Failed to retrieve updated offer: OfferId={OfferId}",
                    offer.Id);
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Offer", new[] { "Failed to retrieve updated offer." } }
                });
            }

            _logger.LogInformation(
                "Offer update completed: OfferId={OfferId}",
                updatedOffer.Id);

            return _mapper.Map<OfferResponseDto>(updatedOffer);
        }

        private void ValidateCommand(UpdateOfferCommand request)
        {
            var errors = new Dictionary<string, string[]>();

            if (request.OfferId <= 0)
            {
                errors.Add("OfferId", new[] { "OfferId must be a positive integer." });
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

            if (request.Message != null && request.Message.Length > 500)
            {
                errors.Add("Message", new[] { "Message cannot exceed 500 characters." });
            }

            if (errors.Any())
            {
                _logger.LogWarning(
                    "Offer update validation failed: OfferId={OfferId}, Errors={Errors}",
                    request.OfferId,
                    string.Join(", ", errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")));
                throw new ValidationException(errors);
            }
        }
    }
}
