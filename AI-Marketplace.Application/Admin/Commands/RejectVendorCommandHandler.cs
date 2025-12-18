using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Commands
{
    public class RejectVendorCommandHandler : IRequestHandler<RejectVendorCommand, String>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductRepository _productRepository;

        public RejectVendorCommandHandler(IStoreRepository storeRepository, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _storeRepository = storeRepository;
            _userManager = userManager;
            _productRepository = productRepository;
        }
        public async Task<string> Handle(RejectVendorCommand request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdAsync(request.StoreId, cancellationToken);
            if (store == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store Not Found." } }
                });
            }
            store.IsVerified = false;
            store.IsActive = false;
            store.VerifiedAt = DateOnly.MinValue;
            store.VerifiedBy = string.Empty;
            await _storeRepository.UpdateAsync(store, cancellationToken);
            var products = await _productRepository.GetByStoreIdAsync(store.Id, cancellationToken);
            foreach (var product in products)
            {
                if (product.IsActive)
                {
                    product.IsActive = false;
                    await _productRepository.UpdateAsync(product, cancellationToken);
                }
            }
            return "Vendor rejected successfully.";
        }
    }
}
