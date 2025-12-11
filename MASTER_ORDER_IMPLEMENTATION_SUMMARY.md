# Master Order Implementation - Complete Summary

## Problem Statement
Users had to make multiple payments when their cart contained items from different stores, because each store created a separate order. This created a poor user experience.

## Solution: Master Order Pattern
Implemented a **Master Order** entity that groups multiple store orders together, allowing a single payment for the entire cart while maintaining separate orders per store for fulfillment.

---

## Architecture Changes

### 1. New Entity: `MasterOrder`
**Location:** `AI-Marketplace.Domain\Entities\MasterOrder.cs`

```csharp
public class MasterOrder
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ShippingAddress { get; set; }
    public int? ShippingAddressId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Navigation
    public ApplicationUser Buyer { get; set; }
    public ICollection<Order> ChildOrders { get; set; }
    public ICollection<Payment> Payments { get; set; }
}
```

### 2. Updated Entities

#### `Order` Entity
**Added:**
- `public int? MasterOrderId { get; set; }`
- `public MasterOrder? MasterOrder { get; set; }`

**Purpose:** Links individual store orders to their parent master order.

#### `Payment` Entity
**Changed:**
- `OrderId` is now **nullable** (`int?`)
- Added `MasterOrderId` (nullable)
- Added navigation properties for both

**Purpose:** Payments can now link to either individual orders OR master orders.

### 3. New Repository
**Interface:** `AI-Marketplace.Application\Common\Interfaces\IMasterOrderRepository.cs`
**Implementation:** `AI-Marketplace.Infrastructure\Repositories\Orders\MasterOrderRepository.cs`

---

## Data Flow

### Before (Old Flow)
```
Cart ? Multiple Orders (Store A, B, C)
     ?
User must create payment for EACH order
     ?
User completes EACH payment separately
```

### After (New Flow)
```
Cart ? MasterOrder
     ?
MasterOrder ? Multiple Orders (Store A, B, C)
     ?
User creates ONE payment for MasterOrder
     ?
User completes payment ONCE
```

---

## API Changes

### 1. Create Orders Endpoint
**Endpoint:** `POST /api/orders/create`

#### Request (Unchanged)
```json
{
  "shippingAddress": "123 Main St, City, Country"
}
```

#### Response (CHANGED)
**Before:**
```json
{
  "data": [
    { "id": 1, "storeId": 10, "totalAmount": 100.00, ... },
    { "id": 2, "storeId": 20, "totalAmount": 50.00, ... }
  ],
  "message": "Orders created successfully.",
  "success": true
}
```

**After:**
```json
{
  "data": {
    "masterOrderId": 123,
    "totalAmount": 150.00,
    "orders": [
      { "id": 1, "masterOrderId": 123, "storeId": 10, "totalAmount": 100.00, ... },
      { "id": 2, "masterOrderId": 123, "storeId": 20, "totalAmount": 50.00, ... }
    ]
  },
  "message": "Orders created successfully.",
  "success": true
}
```

### 2. Create Payment Intent Endpoint
**Endpoint:** `POST /api/payment/create-intent`

#### Request (CHANGED)
**Before:**
```json
{
  "orderId": 1,
  "amount": 100,
  "currencyCode": "USD"
}
```

**After:**
```json
{
  "masterOrderId": 123,
  "amount": 150,
  "currencyCode": "USD"
}
```

#### Response (Unchanged)
```json
{
  "data": "pi_xxxxx_secret_xxxxx",
  "message": "Payment intent created successfully...",
  "success": true
}
```

### 3. Get Payment Endpoints
**Response Updated:**
```json
{
  "id": 1,
  "orderId": null,
  "masterOrderId": 123,
  "paymentIntentId": "pi_xxxxx",
  "amount": 150,
  "currency": "USD",
  "status": "Succeeded",
  ...
}
```

---

## Database Migration Required

### Migration Steps
```bash
# 1. Navigate to API project
cd AI-Marketplace.Api

# 2. Create migration
dotnet ef migrations add AddMasterOrderSupport \
  --project ..\AI-Marketplace.Infrastructure \
  --startup-project .

# 3. Apply migration
dotnet ef database update \
  --project ..\AI-Marketplace.Infrastructure \
  --startup-project .
```

### What the Migration Creates
1. **New table:** `MasterOrders`
   - Columns: Id, BuyerId, TotalAmount, Status, ShippingAddress, ShippingAddressId, CreatedAt, CompletedAt
   
2. **Updated table:** `Orders`
   - New column: `MasterOrderId` (nullable FK to MasterOrders)
   
3. **Updated table:** `Payments`
   - `OrderId` now nullable
   - New column: `MasterOrderId` (nullable FK to MasterOrders)

---

## Frontend Integration

### Updated Checkout Flow

```typescript
// 1. Create orders from cart
async function checkout(shippingAddress: string) {
  const response = await fetch('/api/orders/create', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ shippingAddress })
  });
  
  const result = await response.json();
  const { masterOrderId, totalAmount, orders } = result.data;
  
  console.log(`Created ${orders.length} orders under master order ${masterOrderId}`);
  
  return { masterOrderId, totalAmount };
}

// 2. Create payment intent
async function createPaymentIntent(masterOrderId: number, totalAmount: number) {
  const response = await fetch('/api/payment/create-intent', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      masterOrderId: masterOrderId,
      amount: totalAmount,
      currencyCode: 'USD'
    })
  });
  
  const result = await response.json();
  return result.data; // clientSecret
}

// 3. Complete payment (Stripe.js - no changes)
async function completePayment(clientSecret: string, cardElement: any) {
  const stripe = Stripe('your_publishable_key');
  const { paymentIntent, error } = await stripe.confirmCardPayment(clientSecret, {
    payment_method: {
      card: cardElement,
      billing_details: { name: 'Customer Name' }
    }
  });
  
  if (error) {
    console.error('Payment failed:', error.message);
  } else {
    console.log('Payment succeeded:', paymentIntent.id);
  }
}

// Complete flow
async function handleCheckout(shippingAddress: string, cardElement: any) {
  try {
    // Step 1: Create orders
    const { masterOrderId, totalAmount } = await checkout(shippingAddress);
    
    // Step 2: Create payment intent
    const clientSecret = await createPaymentIntent(masterOrderId, totalAmount);
    
    // Step 3: Complete payment
    await completePayment(clientSecret, cardElement);
    
    console.log('Checkout complete!');
  } catch (error) {
    console.error('Checkout failed:', error);
  }
}
```

---

## Benefits

### User Experience
? **Single payment** for entire cart regardless of number of stores
? **Simplified checkout** process
? **Clear total** upfront

### Business Logic
? **Store separation maintained** - each store still gets their own order
? **Independent fulfillment** - stores can ship separately
? **Better tracking** - master order groups related orders

### Technical
? **Backward compatible** - individual orders (e.g., from offers) still work
? **Clean architecture** - follows existing patterns
? **Extensible** - easy to add master order queries/features later

---

## Testing Checklist

### Database
- [ ] Migration applied successfully
- [ ] MasterOrders table created
- [ ] Foreign keys working correctly

### API Endpoints
- [ ] `POST /api/orders/create` returns CreateOrdersResponse with masterOrderId
- [ ] `POST /api/payment/create-intent` accepts masterOrderId
- [ ] Payment creation works with master order
- [ ] Individual orders still have masterOrderId populated

### Payment Flow
- [ ] Can create payment intent with master order
- [ ] Can complete payment via Stripe
- [ ] Payment record shows masterOrderId
- [ ] All child orders show correct masterOrderId

### Edge Cases
- [ ] Cart with single store (still creates master order)
- [ ] Empty cart validation still works
- [ ] Offer-based orders (non-cart) still work without master order

---

## Files Changed/Created

### Created Files
1. `AI-Marketplace.Domain\Entities\MasterOrder.cs`
2. `AI-Marketplace.Application\Common\Interfaces\IMasterOrderRepository.cs`
3. `AI-Marketplace.Infrastructure\Repositories\Orders\MasterOrderRepository.cs`
4. `AI-Marketplace.Application\Orders\DTOs\MasterOrderDto.cs`
5. `AI-Marketplace.Application\Orders\DTOs\CreateOrdersResponse.cs`
6. `MASTER_ORDER_MIGRATION_GUIDE.md`

### Modified Files
1. `AI-Marketplace.Domain\Entities\Order.cs` - Added MasterOrderId
2. `AI-Marketplace.Domain\Entities\Payment.cs` - Made OrderId nullable, added MasterOrderId
3. `AI-Marketplace.Infrastructure\Data\ApplicationDbContext.cs` - Added MasterOrder configuration
4. `AI-Marketplace.Infrastructure\DependencyInjection.cs` - Registered IMasterOrderRepository
5. `AI-Marketplace.Application\Orders\Commands\CreateOrdersFromCartCommand.cs` - Return type changed
6. `AI-Marketplace.Application\Orders\Commands\CreateOrdersFromCartCommandHandler.cs` - Creates MasterOrder
7. `AI-Marketplace.Application\Orders\DTOs\OrderDto.cs` - Added MasterOrderId property
8. `AI-Marketplace.Application\Payment\DTOs\PaymentDto.cs` - Changed to MasterOrderId
9. `AI-Marketplace.Application\Payment\DTOs\PaymentResponseDto.cs` - Added MasterOrderId, made OrderId nullable
10. `AI-Marketplace.Application\Payment\Commands\CreatePaymentIntentCommand.cs` - Changed to MasterOrderId
11. `AI-Marketplace.Application\Payment\Commands\CreatePaymentIntentCommandHandler.cs` - Uses MasterOrderRepository
12. `AI-Marketplace.Api\Controllers\PaymentController.cs` - Updated to use MasterOrderId
13. Payment query handlers (3 files) - Updated to include MasterOrderId in responses

---

## Rollback Instructions

If you need to revert these changes:

```bash
# 1. Revert database migration
dotnet ef database update <PreviousMigrationName> \
  --project ..\AI-Marketplace.Infrastructure \
  --startup-project .

# 2. Remove migration file
dotnet ef migrations remove \
  --project ..\AI-Marketplace.Infrastructure \
  --startup-project .

# 3. Revert code changes using Git
git revert <commit-hash>
```

---

## Next Steps (Optional Enhancements)

Consider implementing these features in future iterations:

1. **Master Order Queries**
   - GET /api/master-orders/{id}
   - GET /api/master-orders/user (get user's master orders)

2. **Master Order Status Management**
   - Update master order status when all child orders complete
   - Cancel master order (cascades to child orders)

3. **Enhanced Payment Tracking**
   - Link partial refunds to specific child orders
   - Split refund across multiple stores

4. **Analytics**
   - Track average order value by master order
   - Multi-store purchase frequency

---

## Support

For questions or issues:
1. Check the migration guide: `MASTER_ORDER_MIGRATION_GUIDE.md`
2. Review build errors in Visual Studio
3. Check database schema after migration
4. Test with Postman/Swagger before frontend integration
