using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.CustomRequests.Commands
{
    public class DeleteCustomRequestCommandHandler : IRequestHandler<DeleteCustomRequestCommand, bool>
    {
        private readonly ICustomRequestRepository _customRequestRepository;

        public DeleteCustomRequestCommandHandler(ICustomRequestRepository customRequestRepository)
        {
            _customRequestRepository = customRequestRepository;
        }

        public async Task<bool> Handle(DeleteCustomRequestCommand request, CancellationToken cancellationToken)
        {
            
            var customRequest = await _customRequestRepository.GetByIdAsync(request.Id, cancellationToken);
            if (customRequest == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "CustomRequest", new[] { $"Custom request with ID {request.Id} not found." } }
                });
            }

            
            if (customRequest.BuyerId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own custom requests.");
            }

            // Can't delete if offers are accepted
            if (customRequest.Offers.Any(o => o.Status == "Accepted"))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Offers", new[] { "Cannot delete custom request with accepted offers." } }
                });
            }

            // check for InProgress or Completed 
            if (customRequest.Status == CustomRequestStatus.InProgress ||
                customRequest.Status == CustomRequestStatus.Completed)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Status", new[] { "Cannot delete a custom request that is in progress or completed." } }
                });
            }


            await _customRequestRepository.DeleteAsync(request.Id, cancellationToken);

            return true;
        }
    }
}
