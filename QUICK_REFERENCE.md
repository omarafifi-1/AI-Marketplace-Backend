# Quick Reference: Master Order Implementation

## What Was the Problem?
Cart items from multiple stores created separate orders, requiring users to pay multiple times.

## What's the Solution?
A `MasterOrder` entity groups all orders from a single checkout, allowing **one payment** for the entire cart.

---

## Quick Start

### 1. Run Database Migration (REQUIRED!)
```bash
cd AI-Marketplace.Api
dotnet ef migrations add AddMasterOrderSupport --project ..\AI-Marketplace.Infrastructure --startup-project .
dotnet ef database update --project ..\AI-Marketplace.Infrastructure --startup-project .
```

### 2. Update Frontend Checkout
**Before:**
```javascript
// Old: Had to specify orderId
const payment = await createPayment({ 
  orderId: 1, 
  amount: 100 
});
```

**After:**
```javascript
// Step 1: Create orders (returns masterOrderId)
const checkout = await createOrders({ shippingAddress: "..." });
// checkout.data = { masterOrderId: 123, totalAmount: 150, orders: [...] }

// Step 2: Create payment with masterOrderId
const payment = await createPayment({ 
  masterOrderId: checkout.data.masterOrderId,
  amount: checkout.data.totalAmount,
  currencyCode: "USD"
});

// Step 3: Complete payment (same as before)
stripe.confirmCardPayment(payment.data, ...);
```

---

## Key Changes

| Component | Change |
|-----------|--------|
| **Database** | New `MasterOrders` table + nullable FKs |
| **Order Entity** | Added `MasterOrderId` property |
| **Payment Entity** | `OrderId` nullable, added `MasterOrderId` |
| **Create Orders API** | Returns `{ masterOrderId, totalAmount, orders }` |
| **Payment API** | Accepts `masterOrderId` instead of `orderId` |

---

## Data Flow

```
Cart Items
    ?
MasterOrder (1) ? Payment links here
    ?
?? Order 1 (Store A)
?? Order 2 (Store B)  
?? Order 3 (Store C)
```

**User pays once, stores fulfill independently.**

---

## Testing Commands

```bash
# Build the project
dotnet build

# Run migrations
dotnet ef database update --project AI-Marketplace.Infrastructure --startup-project AI-Marketplace.Api

# Test the API
# POST /api/orders/create
# Should return: { masterOrderId: X, totalAmount: Y, orders: [...] }

# POST /api/payment/create-intent
# Should accept: { masterOrderId: X, amount: Y, currencyCode: "USD" }
```

---

## Complete Documentation

For detailed information, see:
- **Implementation Details**: `MASTER_ORDER_IMPLEMENTATION_SUMMARY.md`
- **Migration Guide**: `MASTER_ORDER_MIGRATION_GUIDE.md`
