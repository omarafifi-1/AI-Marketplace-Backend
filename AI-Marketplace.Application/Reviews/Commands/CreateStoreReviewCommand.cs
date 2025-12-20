using AI_Marketplace.Application.Reviews.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Reviews.Commands
{
    public class CreateStoreReviewCommand : IRequest<int>
    {
        public StoreReviewDto StoreReview { get; }

        public CreateStoreReviewCommand(StoreReviewDto storeReview)
        {
            StoreReview = storeReview;
        }
    }
}