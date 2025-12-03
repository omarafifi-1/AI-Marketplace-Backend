using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Offers.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Offers.Queries
{
    public class GetOfferByIdQueryHandler : IRequestHandler<GetOfferByIdQuery, OfferResponseDto?>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOfferByIdQueryHandler> _logger;

        public GetOfferByIdQueryHandler(
            IOfferRepository offerRepository,
            IMapper mapper,
            ILogger<GetOfferByIdQueryHandler> logger)
        {
            _offerRepository = offerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OfferResponseDto?> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving offer by ID: OfferId={OfferId}",
                request.OfferId);

            // Call repository to get offer by ID
            var offer = await _offerRepository.GetByIdAsync(request.OfferId, cancellationToken);

            // Return null if not found
            if (offer == null)
            {
                _logger.LogWarning(
                    "Offer not found: OfferId={OfferId}",
                    request.OfferId);
                return null;
            }

            _logger.LogInformation(
                "Offer retrieved successfully: OfferId={OfferId}, StoreId={StoreId}, Status={Status}",
                offer.Id,
                offer.StoreId,
                offer.Status);

            // Map to DTO
            var offerDto = _mapper.Map<OfferResponseDto>(offer);

            return offerDto;
        }
    }
}
