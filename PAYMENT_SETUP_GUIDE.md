# Payment Entity Setup - AI Marketplace

## Overview
This document outlines the complete Payment entity setup for the AI Marketplace, supporting multiple payment methods, refunds, and transaction tracking.

## ?? IMPORTANT UPDATE
**PaymentIntentId vs TransactionId:**
- `PaymentIntentId` (pi_xxxxx) - Set when payment is **initiated**
- `TransactionId` (ch_xxxxx) - Set when payment **succeeds** (actual charge)
- See `PAYMENT_INTENT_FIX_SUMMARY.md` for detailed explanation

## Database Schema

### Payment Table Structure
- **Id**: Primary key
- **OrderId**: Foreign key to Order
- **PaymentMethod**: Enum (Stripe, PayPal, CreditCard, DebitCard, BankTransfer, Wallet, CashOnDelivery)
- **PaymentIntentId**: Stripe Payment Intent ID (pi_xxxxx) - Set at payment creation ?
- **TransactionId**: Stripe Charge ID (ch_xxxxx) - Set when payment succeeds ?
- **PaymentGatewayResponse**: JSON response from payment gateway
- **Amount**: Payment amount in smallest currency unit (cents)
- **Currency**: 3-letter currency code (e.g., USD)
- **Status**: Enum (Pending, Processing, Succeeded, Failed, Cancelled, Refunded, PartiallyRefunded)
- **FailureReason**: Reason for payment failure
- **RefundedAmount**: Amount refunded in smallest currency unit
- **RefundTransactionId**: Transaction ID from refund operation
- **RefundedAt**: Timestamp of refund
- **RefundReason**: Reason for refund
- **CustomerEmail**: Customer's email address
- **CustomerName**: Customer's full name
- **BillingAddress**: Billing address
- **CreatedAt**: Payment creation timestamp
- **ProcessedAt**: Payment processing timestamp
- **CompletedAt**: Payment completion timestamp

## Files Created

### Domain Layer
1. `AI-Marketplace.Domain/Entities/Payment.cs` - Payment entity
2. `AI-Marketplace.Domain/enums/PaymentMethod.cs` - Payment method enum
3. `AI-Marketplace.Domain/enums/PaymentStatus.cs` - Payment status enum

### Application Layer
4. `AI-Marketplace.Application/Common/Interfaces/IPaymentRepository.cs` - Repository interface
5. `AI-Marketplace.Application/Common/Interfaces/IStripePaymentService.cs` - Stripe service interface with DTOs
6. `AI-Marketplace.Application/Payment/DTOs/PaymentResponseDto.cs` - Payment response DTO
7. `AI-Marketplace.Application/Payment/Commands/ConfirmPaymentCommand.cs` - Confirm payment command
8. `AI-Marketplace.Application/Payment/Commands/ConfirmPaymentCommandHandler.cs` - Confirm payment handler
9. `AI-Marketplace.Application/Payment/Commands/RefundPaymentCommand.cs` - Refund payment command
10. `AI-Marketplace.Application/Payment/Commands/RefundPaymentCommandHandler.cs` - Refund payment handler
11. `AI-Marketplace.Application/Payment/Queries/GetPaymentByIdQuery.cs` - Get payment by ID query
12. `AI-Marketplace.Application/Payment/Queries/GetPaymentByIdQueryHandler.cs` - Get payment by ID handler
13. `AI-Marketplace.Application/Payment/Queries/GetPaymentsByOrderIdQuery.cs` - Get payments by order query
14. `AI-Marketplace.Application/Payment/Queries/GetPaymentsByOrderIdQueryHandler.cs` - Get payments by order handler

### Infrastructure Layer
15. `AI-Marketplace.Infrastructure/Repositories/Payments/PaymentRepository.cs` - Payment repository implementation
16. `AI-Marketplace.Infrastructure/ExternalServices/payment/StripePaymentService.cs` - Stripe integration
17. Updated `AI-Marketplace.Infrastructure/Data/ApplicationDbContext.cs` - Added Payment DbSet and configuration
18. Updated `AI-Marketplace.Infrastructure/DependencyInjection.cs` - Registered PaymentRepository

### Updates to Existing Files
19. Updated `AI-Marketplace.Domain/Entities/Order.cs` - Added Payments navigation property
20. Updated `AI-Marketplace.Application/Payment/Commands/CreatePaymentIntentCommand.cs` - Added OrderId
21. Updated `AI-Marketplace.Application/Payment/Commands/CreatePaymentIntentCommandHandler.cs` - Creates Payment record with PaymentIntentId

## Features

### 1. Payment Creation
When a payment intent is created via Stripe:
- A Payment record is automatically created in the database with status `Pending`
- The payment is linked to the Order
- **PaymentIntentId** is stored from Stripe (pi_xxxxx)
- Customer information is captured from the Order's Buyer

### 2. Payment Confirmation
Use `ConfirmPaymentCommand` to mark a payment as succeeded:
- Finds payment by **PaymentIntentId**
- Updates payment status to `Succeeded`
- Sets **TransactionId** (Charge ID from Stripe)
- Updates Order status to `Processing`
- Records completion timestamps

### 3. Refunds
Use `RefundPaymentCommand` to process refunds:
- Supports full and partial refunds
- Automatically updates payment status (Refunded or PartiallyRefunded)
- Tracks refund amount, transaction ID, and reason

### 4. Payment Queries
- Get payment by ID
- Get payment by PaymentIntentId
- Get payment by TransactionId
- Get all payments for an order
- Get payments by status (Pending, Failed, etc.)

## Next Steps

### 1. Create Database Migration
Run this command to create the database migration:
```bash
dotnet ef migrations add AddPaymentIntentIdToPayment --project AI-Marketplace.Infrastructure --startup-project AI-Marketplace.Api
```

Then apply the migration:
```bash
dotnet ef database update --project AI-Marketplace.Infrastructure --startup-project AI-Marketplace.Api
```

### 2. Implement Stripe Webhooks
Create a webhook endpoint to handle Stripe events:
- `payment_intent.succeeded` - Call ConfirmPaymentCommand with PaymentIntentId
- `payment_intent.payment_failed` - Mark payment as failed
- `charge.refunded` - Update refund information

### 3. Update PaymentController
Add endpoints for:
- GET `/api/payment/{id}` - Get payment details ?
- GET `/api/payment/order/{orderId}` - Get all payments for an order ?
- POST `/api/payment/confirm` - Confirm payment ?
- POST `/api/payment/refund` - Process a refund (admin only) ?
- POST `/api/payment/webhook` - Handle Stripe webhooks (TODO)

### 4. Enhance StripePaymentService
Add methods for:
- ? Retrieving payment intent status
- TODO: Processing refunds via Stripe API
- TODO: Handling payment failures

### 5. Add Authorization
Implement proper role-based authorization:
- Buyers can view their own payments
- Vendors can view payments for their orders
- Admins can view all payments and process refunds

## Usage Example

### Creating a Payment Intent for an Order
```csharp
var command = new CreatePaymentIntentCommand
{
    OrderId = 123,
    Amount = 5000, // $50.00 in cents
    Currency = "usd"
};

var clientSecret = await mediator.Send(command);
// Returns clientSecret for Stripe.js
// PaymentIntentId is automatically stored in database
```

### Confirming a Payment (from webhook or frontend)
```csharp
var command = new ConfirmPaymentCommand(
    transactionId: "pi_1234567890",  // This is PaymentIntentId for lookup
    paymentIntentId: "ch_9876543210"  // This is ChargeId to store as TransactionId
);

var result = await mediator.Send(command);
```

### Processing a Refund
```csharp
var command = new RefundPaymentCommand(
    paymentId: 1,
    refundAmount: 5000, // Full refund
    refundReason: "Customer requested refund",
    userId: currentUserId
);

var result = await mediator.Send(command);
```

## Database Indexes
The following indexes are created for performance:
- `PaymentIntentId` - For quick lookup by Stripe Payment Intent ID ?
- `TransactionId` - For quick lookup by Stripe Charge/Transaction ID
- `OrderId` - For retrieving all payments for an order

## Benefits of This Setup

1. **Complete Audit Trail**: Every payment transaction is tracked with timestamps
2. **Clear ID Separation**: PaymentIntentId (intent) vs TransactionId (charge) ?
3. **Multi-Payment Support**: Orders can have multiple payment attempts
4. **Refund Tracking**: Full support for partial and complete refunds
5. **Payment Method Flexibility**: Ready to add PayPal, bank transfers, etc.
6. **Failure Analysis**: Track and analyze payment failures
7. **Financial Reporting**: Easy to generate reports on revenue, refunds, etc.
8. **Customer Service**: Quick lookup of payment history for support
9. **Webhook Idempotency**: Use PaymentIntentId to prevent duplicate processing ?

## Important Notes

- Amounts are stored in the **smallest currency unit** (cents for USD) to avoid floating-point issues
- Payment records are **never deleted**, only status is updated for audit purposes
- The **PaymentIntentId** is set when payment is created (Stripe's pi_xxxxx)
- The **TransactionId** is set when payment succeeds (Stripe's ch_xxxxx)
- Use `DeleteBehavior.Restrict` on Order relationship to prevent accidental deletion
- The Application layer uses DTOs (`PaymentIntentResult`, `StripePaymentIntentInfo`) to avoid direct dependency on Stripe SDK

## Payment Flow Diagram

```
1. Customer clicks "Checkout"
   ?
2. Backend creates Order (PendingPayment) + Payment (Pending, PaymentIntentId=pi_xxx)
   ?
3. Return clientSecret to frontend
   ?
4. Frontend uses Stripe.js to process payment
   ?
5. Stripe webhook or frontend confirms ? Backend finds payment by pi_xxx
   ?
6. Backend sets TransactionId=ch_xxx, Status=Succeeded
   ?
7. Order status updated to Pending/Processing
   ?
8. Cart cleared
