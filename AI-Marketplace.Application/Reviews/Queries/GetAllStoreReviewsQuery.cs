using AI_Marketplace.Application.Reviews.DTOs;
using MediatR;
using System.Collections.Generic;

namespace AI_Marketplace.Application.Reviews.Queries
{
    public class GetAllStoreReviewsQuery : IRequest<List<ReviewDto>>
    {
        public int StoreId { get; }

        public GetAllStoreReviewsQuery(int storeId)
        {
            StoreId = storeId;
        }
    }
}
