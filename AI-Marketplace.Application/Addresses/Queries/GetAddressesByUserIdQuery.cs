using AI_Marketplace.Application.Addresses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Queries
{
    public class GetAddressesByUserIdQuery : IRequest<IEnumerable<AddressResponseDto>>
    {
        public int UserId { get; }

        public GetAddressesByUserIdQuery(int userId)
        {
            UserId = userId;
        }
    }
}
