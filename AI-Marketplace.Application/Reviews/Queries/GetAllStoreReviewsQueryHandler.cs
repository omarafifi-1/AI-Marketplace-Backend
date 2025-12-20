using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Reviews.DTOs;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Reviews.Queries
{
    public class GetAllStoreReviewsQueryHandler : IRequestHandler<GetAllStoreReviewsQuery, List<ReviewDto>>
    {
        private readonly IReviewsRepository _reviewsRepository;

        public GetAllStoreReviewsQueryHandler(IReviewsRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        public async Task<List<ReviewDto>> Handle(GetAllStoreReviewsQuery request, CancellationToken cancellationToken)
        {
            return await _reviewsRepository.GetAllStoreReviewsAsync(request.StoreId, cancellationToken);
        }
    }
}
