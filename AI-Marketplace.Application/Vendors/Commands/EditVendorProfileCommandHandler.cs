using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Vendors.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Commands
{
    public class EditVendorProfileCommandHandler : IRequestHandler<EditVendorProfileCommand, VendorProfileDto>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public EditVendorProfileCommandHandler(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }
        public async Task<VendorProfileDto> Handle(EditVendorProfileCommand request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);
            if (store == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store not found for the given user." } }
                });
            }
            store = _mapper.Map(request.VendorEditDto, store);
            store.UpdatedAt = DateTime.UtcNow;
            await _storeRepository.UpdateAsync(store, cancellationToken);
            return _mapper.Map<VendorProfileDto>(store);
        }
    }
}
