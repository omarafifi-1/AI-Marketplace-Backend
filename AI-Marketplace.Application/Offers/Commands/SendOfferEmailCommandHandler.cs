using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class SendOfferEmailCommandHandler : IRequestHandler<SendOfferEmailCommand, bool>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<SendOfferEmailCommandHandler> _logger;

        public SendOfferEmailCommandHandler(
            IOfferRepository offerRepository,
            ICustomRequestRepository customRequestRepository,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<SendOfferEmailCommandHandler> logger)
        {
            _offerRepository = offerRepository;
            _customRequestRepository = customRequestRepository;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<bool> Handle(SendOfferEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting email sending for accepted offer: OfferId={OfferId}, UserId={UserId}",
                request.OfferId,
                request.UserId);

            // 1. Retrieve offer with navigation properties
            var offer = await _offerRepository.GetByIdAsync(request.OfferId, cancellationToken);
            if (offer == null)
            {
                _logger.LogWarning("Offer not found: OfferId={OfferId}", request.OfferId);
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Offer", new[] { $"Offer with ID {request.OfferId} not found." } }
                });
            }

            // 2. Verify offer status is "Accepted"
            if (offer.Status != "Accepted")
            {
                _logger.LogWarning(
                    "Cannot send email for non-accepted offer: OfferId={OfferId}, Status={Status}",
                    request.OfferId,
                    offer.Status);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Offer", new[] { $"Only accepted offers can trigger email notifications. Current status: {offer.Status}" } }
                });
            }

            // 3. Verify authenticated user owns the store that made the offer
            var seller = await _userManager.Users
                .Include(u => u.Store)
                .FirstOrDefaultAsync(u => u.Id == request.UserId);
                
            if (seller?.Store == null || seller.Store.Id != offer.StoreId)
            {
                _logger.LogWarning(
                    "Unauthorized email send attempt: UserId={UserId}, OfferStoreId={StoreId}, UserStoreId={UserStoreId}",
                    request.UserId,
                    offer.StoreId,
                    seller?.Store?.Id);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Authorization", new[] { "Only the seller who created the offer can send emails." } }
                });
            }

            // 4. Retrieve custom request and buyer information
            var customRequest = await _customRequestRepository.GetByIdAsync(offer.CustomRequestId, cancellationToken);
            if (customRequest == null)
            {
                _logger.LogError(
                    "CustomRequest not found for offer: CustomRequestId={CustomRequestId}",
                    offer.CustomRequestId);
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "CustomRequest", new[] { "Custom request not found." } }
                });
            }

            // 5. Get buyer email
            var buyer = await _userManager.FindByIdAsync(customRequest.BuyerId.ToString());
            if (buyer == null || string.IsNullOrEmpty(buyer.Email))
            {
                _logger.LogError(
                    "Buyer not found or has no email: BuyerId={BuyerId}",
                    customRequest.BuyerId);
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Buyer", new[] { "Buyer not found or has no email address." } }
                });
            }

            // 6. Compose and send email
            var subject = $"Your Custom Request Product is Ready - {seller.Store.StoreName}";
            var htmlBody = $@"
                <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9fafb;"">
                    <div style=""background-color: white; border-radius: 8px; padding: 30px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);"">
                        <h2 style=""color: #4f46e5; margin-top: 0;"">Good News About Your Custom Request!</h2>
                        
                        <p style=""color: #374151; font-size: 16px; line-height: 1.6;"">
                            Hello {buyer.FirstName ?? "Valued Customer"},
                        </p>
                        
                        <p style=""color: #374151; font-size: 16px; line-height: 1.6;"">
                            Great news! <strong>{seller.Store.StoreName}</strong> has added the custom product from your request to their store.
                        </p>
                        
                        <div style=""background-color: #f3f4f6; border-left: 4px solid #4f46e5; padding: 15px; margin: 20px 0; border-radius: 4px;"">
                            <h3 style=""margin: 0 0 10px 0; color: #1f2937; font-size: 16px;"">Offer Details:</h3>
                            <p style=""margin: 5px 0; color: #4b5563;""><strong>Price:</strong> EGP {offer.ProposedPrice:F2}</p>
                            <p style=""margin: 5px 0; color: #4b5563;""><strong>Estimated Delivery:</strong> {offer.EstimatedDays} days</p>
                            <p style=""margin: 5px 0; color: #4b5563;""><strong>Custom Request ID:</strong> #{customRequest.Id}</p>
                        </div>
                        
                        <p style=""color: #374151; font-size: 16px; line-height: 1.6;"">
                            You can now visit <strong>{seller.Store.StoreName}</strong>'s store to purchase this custom product at your convenience.
                        </p>
                        
                        <div style=""margin-top: 30px; padding-top: 20px; border-top: 1px solid #e5e7eb;"">
                            <p style=""color: #6b7280; font-size: 14px; margin: 0;"">
                                Thanks for using Bazario!
                            </p>
                        </div>
                    </div>
                </div>
            ";

            try
            {
                await _emailService.SendEmailAsync(buyer.Email, subject, htmlBody);
                
                _logger.LogInformation(
                    "Email sent successfully: OfferId={OfferId}, BuyerEmail={Email}",
                    request.OfferId,
                    buyer.Email);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email: OfferId={OfferId}, BuyerEmail={Email}",
                    request.OfferId,
                    buyer.Email);
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Failed to send email. Please try again later." } }
                });
            }
        }
    }
}
