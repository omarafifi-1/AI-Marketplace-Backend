using AI_Marketplace.Application.Common.Interfaces;
using MediatR;

namespace AI_Marketplace.Application.Reviews.Commands
{
    public class CreateStoreReviewCommandHandler : IRequestHandler<CreateStoreReviewCommand, int>
    {
        private readonly IReviewsRepository _reviewsRepository;

        public CreateStoreReviewCommandHandler(IReviewsRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        public async Task<int> Handle(CreateStoreReviewCommand request, CancellationToken cancellationToken)
        {
            return await _reviewsRepository.AddStoreReviewAsync(request.StoreReview, cancellationToken);
        }
    }
}