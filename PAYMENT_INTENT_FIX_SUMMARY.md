# Payment Entity Fix - PaymentIntentId vs TransactionId

## What Was Fixed

### Problem
Previously, the Payment entity only had a `TransactionId` field which was being used to store the Stripe Payment Intent ID. This caused confusion because:
- **Payment Intent ID** (`pi_xxxxx`) is created when initiating a payment
- **Charge/Transaction ID** (`ch_xxxxx`) is created when the payment actually succeeds

### Solution
Separated the two IDs for proper tracking:

```csharp
public class Payment
{
    public string? PaymentIntentId { get; set; }  // pi_xxxxx - Created at payment initiation
    public string? TransactionId { get; set; }     // ch_xxxxx - Created when payment succeeds
}
```

## Files Modified

### 1. Domain Layer
- ? `AI-Marketplace.Domain/Entities/Payment.cs` - Already had PaymentIntentId field

### 2. Infrastructure Layer
- ? `AI-Marketplace.Infrastructure/Data/ApplicationDbContext.cs` - Added PaymentIntentId configuration and index
- ? `AI-Marketplace.Infrastructure/ExternalServices/payment/StripePaymentService.cs` - Returns both ClientSecret and PaymentIntentId

### 3. Application Layer
- ? `AI-Marketplace.Application/Common/Interfaces/IStripePaymentService.cs` - New `PaymentIntentResult` DTO
- ? `AI-Marketplace.Application/Common/Interfaces/IPaymentRepository.cs` - Added `GetByPaymentIntentIdAsync`
- ? `AI-Marketplace.Application/Payment/Commands/CreatePaymentIntentCommandHandler.cs` - Stores PaymentIntentId
- ? `AI-Marketplace.Application/Payment/Commands/ConfirmPaymentCommandHandler.cs` - Uses PaymentIntentId for lookup
- ? `AI-Marketplace.Application/Payment/DTOs/PaymentResponseDto.cs` - Includes both IDs
- ? `AI-Marketplace.Infrastructure/Repositories/Payments/PaymentRepository.cs` - Added GetByPaymentIntentIdAsync implementation
- ? All query/command handlers - Include PaymentIntentId in responses

## Database Migration Required

Run this command to create and apply the migration:

```bash
# Create migration
dotnet ef migrations add AddPaymentIntentIdToPayment --project AI-Marketplace.Infrastructure --startup-project AI-Marketplace.Api

# Apply migration
dotnet ef database update --project AI-Marketplace.Infrastructure --startup-project AI-Marketplace.Api
```

This will add the `PaymentIntentId` column to the `Payments` table and create an index on it.

## Payment Flow Timeline

### Step 1: Initiate Payment (Frontend calls backend)
```csharp
POST /api/orders/checkout
{
    "shippingAddress": "123 Main St",
    "paymentMethod": "Stripe"
}
```

**Backend creates:**
- Order (status: `PendingPayment`)
- Payment record with:
  - `PaymentIntentId` = `pi_1234567890` ?
  - `TransactionId` = `null` (not charged yet)
  - `Status` = `Pending`

**Returns to frontend:**
```json
{
    "requiresPayment": true,
    "paymentClientSecret": "pi_1234567890_secret_abc123",
    "pendingOrders": [...]
}
```

### Step 2: Frontend Completes Payment
Frontend uses `clientSecret` with Stripe.js to complete payment

### Step 3: Stripe Webhook or Frontend Confirmation
```csharp
POST /api/payment/confirm
{
    "paymentIntentId": "pi_1234567890"
}
```

**Backend:**
1. Finds payment by `PaymentIntentId`
2. Calls Stripe API to get payment status
3. Extracts `LatestChargeId` (ch_xxxxx)
4. Updates payment:
   - `TransactionId` = `ch_1234567890` ?
   - `Status` = `Succeeded`
5. Updates Order status to `Pending` or `Processing`
6. Clears cart

## Key Differences

| Field | Purpose | When Set | Example |
|-------|---------|----------|---------|
| `PaymentIntentId` | Track the payment attempt | When payment is initiated | `pi_3QJ1KaL12345` |
| `TransactionId` | Track the actual charge | When payment succeeds | `ch_3QJ1KaL67890` |
| `RefundTransactionId` | Track refund | When refund is processed | `re_3QJ1KaLabcd` |

## Benefits

? **Clear Separation** - Intent ID vs Charge ID  
? **Better Webhook Handling** - Can match Stripe events to database records  
? **Audit Trail** - Track payment from creation to completion  
? **Idempotency** - Prevent duplicate processing using PaymentIntentId  
? **Debugging** - Both IDs visible in Stripe Dashboard  

## API Response Example

```json
{
    "id": 1,
    "orderId": 123,
    "paymentMethod": "Stripe",
    "paymentIntentId": "pi_3QJ1KaL12345",      // ? Payment Intent ID
    "transactionId": "ch_3QJ1KaL67890",        // ? Charge/Transaction ID
    "amount": 5000,
    "currency": "usd",
    "status": "Succeeded",
    "createdAt": "2024-01-15T10:30:00Z",
    "completedAt": "2024-01-15T10:30:15Z"
}
```

## Testing Checklist

- [ ] Create migration and apply to database
- [ ] Test checkout flow with Stripe payment
- [ ] Verify PaymentIntentId is stored when creating payment
- [ ] Test payment confirmation (webhook or manual)
- [ ] Verify TransactionId is stored when payment succeeds
- [ ] Test refund flow
- [ ] Verify both IDs appear in API responses
- [ ] Test Cash on Delivery (should work as before)

## Stripe Webhook Events to Handle

```csharp
// Future enhancement: Webhook handler
switch (stripeEvent.Type)
{
    case "payment_intent.succeeded":
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        // Find payment by paymentIntent.Id
        // Set TransactionId = paymentIntent.LatestChargeId
        // Update status to Succeeded
        break;
        
    case "payment_intent.payment_failed":
        // Mark payment as failed
        break;
        
    case "charge.refunded":
        var charge = stripeEvent.Data.Object as Charge;
        // Process refund
        break;
}
```
