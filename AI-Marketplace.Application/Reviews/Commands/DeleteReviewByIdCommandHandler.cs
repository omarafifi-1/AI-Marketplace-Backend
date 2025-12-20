using AI_Marketplace.Application.Common.Interfaces;
using MediatR;

namespace AI_Marketplace.Application.Reviews.Commands
{
    public class DeleteReviewByIdCommandHandler : IRequestHandler<DeleteReviewByIdCommand, bool>
    {
        private readonly IReviewsRepository _reviewsRepository;

        public DeleteReviewByIdCommandHandler(IReviewsRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        public async Task<bool> Handle(DeleteReviewByIdCommand request, CancellationToken cancellationToken)
        {
            return await _reviewsRepository.DeleteReviewByIdAsync(request.ReviewId, cancellationToken);
        }
    }
}