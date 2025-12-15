using AI_Marketplace.Application.Addresses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Queries
{
    public class GetAddressByIdQuery : IRequest<CreateAddressDto>
    {
        public int Id { get; }

        public GetAddressByIdQuery(int id)
        {
            Id = id;
        }
    }
}
