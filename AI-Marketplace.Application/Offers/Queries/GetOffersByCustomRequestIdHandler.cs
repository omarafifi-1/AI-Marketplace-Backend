using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Offers.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AI_Marketplace.Application.Offers.Queries.GetOffersByCustomRequestId
{
    public class GetOffersByCustomRequestIdQueryHandler : IRequestHandler<GetOffersByCustomRequestIdQuery, List<OfferResponseDto>>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOffersByCustomRequestIdQueryHandler> _logger;

        public GetOffersByCustomRequestIdQueryHandler(IOfferRepository offerRepository, IMapper mapper, ILogger<GetOffersByCustomRequestIdQueryHandler> logger)
        {
            _offerRepository = offerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<OfferResponseDto>> Handle(GetOffersByCustomRequestIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving offers for CustomRequestId={CustomRequestId}", request.CustomRequestId);

            var offers = await _offerRepository.GetByCustomRequestIdAsync(request.CustomRequestId, cancellationToken);

            _logger.LogInformation("Found {Count} offers for CustomRequestId={CustomRequestId}", offers.Count, request.CustomRequestId);

            var offerDtos = _mapper.Map<List<OfferResponseDto>>(offers);

            return offerDtos;
        }
    }
}