using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Reviews.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Reviews.Queries
{
    public class GetAllProductReviewsQueryHandler : IRequestHandler<GetAllProductReviewsQuery,List<ReviewDto>>
    {
        private readonly IReviewsRepository _reviewsRepo;
        private readonly IMapper _mapper;

        public GetAllProductReviewsQueryHandler(IReviewsRepository reviewsRepo, IMapper mapper)
        {
            _reviewsRepo = reviewsRepo;
            _mapper = mapper;
        }
        public async Task<List<ReviewDto>?> Handle(GetAllProductReviewsQuery request, CancellationToken cancellationToken)
        {
            var productReviews = await _reviewsRepo.GetAllProductReviewsAsync(request.ProductId, cancellationToken);
            return _mapper.Map<List<ReviewDto>>(productReviews);

        }
    }
}
