using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Offers.DTOs;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class AcceptOfferCommandHandler : IRequestHandler<AcceptOfferCommand, OfferResponseDto>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AcceptOfferCommandHandler> _logger;

        public AcceptOfferCommandHandler(
            IOfferRepository offerRepository,
            ICustomRequestRepository customRequestRepository,
            IMapper mapper,
            ILogger<AcceptOfferCommandHandler> logger)
        {
            _offerRepository = offerRepository;
            _customRequestRepository = customRequestRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OfferResponseDto> Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting offer acceptance: OfferId={OfferId}, UserId={UserId}",
                request.OfferId,
                request.UserId);

            // 1. Retrieve offer with navigation properties
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
                "Offer retrieved: OfferId={OfferId}, Status={Status}, StoreId={StoreId}, CustomRequestId={CustomRequestId}",
                offer.Id,
                offer.Status,
                offer.StoreId,
                offer.CustomRequestId);

            // 2. Validate offer status is "Pending"
            if (offer.Status != "Pending")
            {
                _logger.LogWarning(
                    "Offer cannot be accepted - invalid status: OfferId={OfferId}, Status={Status}",
                    offer.Id,
                    offer.Status);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Offer", new[] { $"Only pending offers can be accepted. Current status: {offer.Status}" } }
                });
            }

            // 3. Retrieve custom request to verify buyer
            var customRequest = await _customRequestRepository.GetByIdAsync(offer.CustomRequestId, cancellationToken);
            if (customRequest == null)
            {
                _logger.LogError(
                    "CustomRequest not found for offer: CustomRequestId={CustomRequestId}",
                    offer.CustomRequestId);
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "CustomRequest", new[] { "Custom request not found." } }
                });
            }

            // 4. Verify authenticated user is the buyer
            if (customRequest.BuyerId != request.UserId)
            {
                _logger.LogWarning(
                    "Unauthorized offer acceptance attempt: UserId={UserId}, BuyerId={BuyerId}",
                    request.UserId,
                    customRequest.BuyerId);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Authorization", new[] { "Only the buyer who created the custom request can accept offers." } }
                });
            }

            // 5. Begin database transaction
            try
            {
                _logger.LogInformation(
                    "Starting transaction for offer acceptance: OfferId={OfferId}",
                    request.OfferId);

                // 5a. Update accepted offer status
                offer.Status = "Accepted";
                await _offerRepository.UpdateAsync(offer, cancellationToken);

                _logger.LogInformation(
                    "Offer status updated to Accepted: OfferId={OfferId}",
                    offer.Id);

                // 5b. Get all pending offers for the custom request
                var pendingOffers = await _offerRepository.GetPendingByCustomRequestIdAsync(
                    offer.CustomRequestId,
                    cancellationToken);

                _logger.LogInformation(
                    "Found {Count} pending offers to reject for CustomRequestId={CustomRequestId}",
                    pendingOffers.Count,
                    offer.CustomRequestId);

                // 5c. Update all other pending offers to "Rejected"
                foreach (var pendingOffer in pendingOffers.Where(o => o.Id != offer.Id))
                {
                    pendingOffer.Status = "Rejected";
                    await _offerRepository.UpdateAsync(pendingOffer, cancellationToken);

                    _logger.LogInformation(
                        "Offer rejected: OfferId={OfferId}",
                        pendingOffer.Id);
                }

                // 5d. Update custom request status to "InProgress"
                customRequest.Status = CustomRequestStatus.InProgress;
                customRequest.UpdatedAt = DateTime.UtcNow;
                await _customRequestRepository.UpdateAsync(customRequest, cancellationToken);

                _logger.LogInformation(
                    "CustomRequest status changed to InProgress: CustomRequestId={CustomRequestId}",
                    customRequest.Id);

                _logger.LogInformation(
                    "Transaction completed successfully for offer acceptance: OfferId={OfferId}",
                    offer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Transaction failed during offer acceptance: OfferId={OfferId}",
                    request.OfferId);
                throw;
            }

            // 6. Retrieve the complete offer with all navigation properties for mapping
            var offerWithDetails = await _offerRepository.GetByIdAsync(offer.Id, cancellationToken);
            if (offerWithDetails == null)
            {
                _logger.LogError(
                    "Failed to retrieve accepted offer: OfferId={OfferId}",
                    offer.Id);
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Offer", new[] { "Failed to retrieve accepted offer." } }
                });
            }

            _logger.LogInformation(
                "Offer acceptance completed successfully: OfferId={OfferId}",
                offerWithDetails.Id);

            return _mapper.Map<OfferResponseDto>(offerWithDetails);
        }
    }
}
