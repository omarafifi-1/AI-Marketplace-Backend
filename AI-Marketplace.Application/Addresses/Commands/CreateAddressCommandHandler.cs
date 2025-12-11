using AI_Marketplace.Application.Addresses.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Commands
{
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressResponseDto>
    {
        private readonly IAddressRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CreateAddressCommandHandler(IAddressRepository repo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AddressResponseDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?
        .User?
        .FindFirst("uid")?
        .Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated.");

            int userId = int.Parse(userIdClaim); 

            var address = _mapper.Map<Address>(request.AddressDto);
            address.UserId = userId; 

            var result = await _repo.AddAddressAsync(address);

            return _mapper.Map<AddressResponseDto>(result);
        }
    }
}
