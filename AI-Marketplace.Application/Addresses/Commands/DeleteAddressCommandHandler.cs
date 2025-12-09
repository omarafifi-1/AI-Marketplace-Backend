using AI_Marketplace.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Commands
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, bool>
    {
        private readonly IAddressRepository _repo;

        public DeleteAddressCommandHandler(IAddressRepository repo)
        {
            _repo = repo;
        }
        public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            return await _repo.DeleteAddressAsync(request.Id);
        }
    }
}
