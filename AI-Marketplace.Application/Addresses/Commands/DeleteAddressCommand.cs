using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Commands
{
    public class DeleteAddressCommand : IRequest<bool>
    {
        public int Id { get; }
        public DeleteAddressCommand(int id)
        {
            Id = id;
        }
    }
}
