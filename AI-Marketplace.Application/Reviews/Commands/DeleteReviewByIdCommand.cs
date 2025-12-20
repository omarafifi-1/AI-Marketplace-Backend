using MediatR;

namespace AI_Marketplace.Application.Reviews.Commands
{
    public class DeleteReviewByIdCommand : IRequest<bool>
    {
        public int ReviewId { get; }

        public DeleteReviewByIdCommand(int reviewId)
        {
            ReviewId = reviewId;
        }
    }
}