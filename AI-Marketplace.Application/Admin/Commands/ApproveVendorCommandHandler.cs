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
    public class ApproveVendorCommandHandler : IRequestHandler<ApproveVendorCommand, String>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductRepository _productRepository;

        public ApproveVendorCommandHandler(IStoreRepository storeRepository, UserManager<ApplicationUser> userManager, IProductRepository productRepository) 
        {
            _storeRepository = storeRepository;
            _userManager = userManager;
            _productRepository = productRepository;
        }
        public async Task<string> Handle(ApproveVendorCommand request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdAsync(request.StoreId, cancellationToken);
            if (store == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store Not Found." } }
                });
            }
            store.IsActive = true;
            store.IsVerified = true;
            store.VerifiedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            var user = await _userManager.FindByIdAsync(request.AdminId.ToString());
            if (user == null) 
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "User", new[] { "Admin User Not Found." } }
                });
            }
            store.VerifiedBy = await _userManager.GetUserNameAsync(user);
            await _storeRepository.UpdateAsync(store, cancellationToken);
            var products = await _productRepository.GetByStoreIdAsync(store.Id, cancellationToken);
            foreach (var product in products) 
            {
                if (product.IsActive) 
                {
                    continue;
                }
                product.IsActive = true;
                await _productRepository.UpdateAsync(product, cancellationToken);
            }
            return "Vendor approved successfully.";
        }
    }
}
