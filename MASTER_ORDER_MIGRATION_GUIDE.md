# Master Order Implementation - Migration Guide

## Overview
This implementation adds a **Master Order** pattern to solve the multi-store payment issue. Users can now pay once for their entire cart, while orders are still divided by store.

## What Changed

### 1. New Entity: `MasterOrder`
- Groups multiple orders from a single checkout session
- Contains the total amount across all stores
- Associated with a single payment

### 2. Updated Entities

#### `Order`
- Added `MasterOrderId` (nullable foreign key)
- Orders created from cart now belong to a MasterOrder

#### `Payment`
- Added `MasterOrderId` (nullable foreign key)
- Now supports both `OrderId` (for individual orders) and `MasterOrderId` (for cart checkouts)

### 3. New Flow

**Before:**
```
Cart (Store A + Store B) ? Order A + Order B ? User must pay twice (2 payments)
```

**After:**
```
Cart (Store A + Store B) ? MasterOrder ? Order A + Order B ? User pays once (1 payment)
```

## Required Database Migration

You **MUST** create and run a migration to update the database schema:

```bash
# Navigate to the API project directory
cd AI-Marketplace.Api

# Create the migration
dotnet ef migrations add AddMasterOrderSupport --project ..\AI-Marketplace.Infrastructure --startup-project .

# Apply the migration
dotnet ef database update --project ..\AI-Marketplace.Infrastructure --startup-project .
```

## API Changes

### 1. Create Orders Endpoint Response Changed
**Endpoint:** `POST /api/orders/create`

**Old Response:**
```json
{
  "data": [
    { "id": 1, "totalAmount": 100 },
    { "id": 2, "totalAmount": 50 }
  ]
}
```

**New Response:**
```json
{
  "data": {
    "masterOrderId": 123,
    "totalAmount": 150,
    "orders": [
      { "id": 1, "masterOrderId": 123, "totalAmount": 100 },
      { "id": 2, "masterOrderId": 123, "totalAmount": 50 }
    ]
  }
}
```

### 2. Payment Intent Endpoint Changed
**Endpoint:** `POST /api/payment/create-intent`

**Old Request:**
```json
{
  "orderId": 1,
  "amount": 100,
  "currencyCode": "USD"
}
```

**New Request:**
```json
{
  "masterOrderId": 123,
  "amount": 150,
  "currencyCode": "USD"
}
```

## Frontend Integration Guide

### Step 1: Create Orders from Cart
```typescript
const response = await fetch('/api/orders/create', {
  method: 'POST',
  body: JSON.stringify({ shippingAddress: '123 Main St' })
});

const result = await response.json();
const masterOrderId = result.data.masterOrderId;
const totalAmount = result.data.totalAmount;
```

### Step 2: Create Single Payment Intent
```typescript
const paymentResponse = await fetch('/api/payment/create-intent', {
  method: 'POST',
  body: JSON.stringify({
    masterOrderId: masterOrderId,
    amount: totalAmount,
    currencyCode: 'USD'
  })
});

const { data: clientSecret } = await paymentResponse.json();
```

### Step 3: Use Stripe.js (no changes needed)
```typescript
const stripe = Stripe('your_publishable_key');
const { paymentIntent, error } = await stripe.confirmCardPayment(clientSecret, {
  payment_method: {
    card: cardElement,
    billing_details: { name: 'Customer Name' }
  }
});
```

## Benefits

1. ? **Single Payment**: User pays once for entire cart
2. ? **Store Separation**: Orders still divided by store for fulfillment
3. ? **Better UX**: Simplified checkout process
4. ? **Backward Compatible**: Individual orders (offers) still work as before
5. ? **Clear Tracking**: MasterOrder groups related orders together

## Testing Checklist

- [ ] Run database migration
- [ ] Test creating orders from cart (should return masterOrderId)
- [ ] Test payment intent creation with masterOrderId
- [ ] Test completing payment
- [ ] Verify orders are properly linked to master order
- [ ] Verify payment is linked to master order
- [ ] Test individual order creation (offers) still works

## Rollback Plan

If you need to rollback:

```bash
# Revert the migration
dotnet ef database update <PreviousMigrationName> --project ..\AI-Marketplace.Infrastructure --startup-project .

# Remove the migration
dotnet ef migrations remove --project ..\AI-Marketplace.Infrastructure --startup-project .
```
