# Webhook Quick Reference

## Endpoint
```
POST /api/webhook/stripe
```

## Supported Events
| Event Type | Action | Payment Status | Order Status Change |
|------------|--------|----------------|---------------------|
| `payment_intent.succeeded` | Confirm payment | `Pending` ? `Succeeded` | `PendingPayment` ? `Pending` |
| `payment_intent.payment_failed` | Mark failed | `Pending` ? `Failed` | `PendingPayment` ? `Cancelled` |
| `charge.refunded` | Process refund | `Succeeded` ? `Refunded`/`PartiallyRefunded` | No change |

## Request Headers
```
Content-Type: application/json
Stripe-Signature: t=1234567890,v1=abc123...
```

## Security
- ? Signature verification enabled
- ? No authentication required (handled by Stripe signature)
- ? HTTPS required in production

## Configuration Required
```json
{
  "Stripe": {
    "WebhookSecret": "whsec_..."
  }
}
```

## Testing with Stripe CLI
```bash
# Forward webhooks to local dev
stripe listen --forward-to https://localhost:5001/api/webhook/stripe

# Trigger test events
stripe trigger payment_intent.succeeded
stripe trigger payment_intent.payment_failed
stripe trigger charge.refunded
```

## Expected Response
```
HTTP 200 OK
```

## Error Responses
| Status Code | Reason |
|-------------|--------|
| 400 | Missing Stripe signature or verification failed |
| 500 | Internal server error processing webhook |

## What Gets Updated

### payment_intent.succeeded
```csharp
Payment:
  - Status: Succeeded
  - TransactionId: ch_xxx (Charge ID)
  - ProcessedAt: Current UTC time
  - CompletedAt: Current UTC time
  - PaymentGatewayResponse: Full Stripe response

Order (if status is PendingPayment):
  - Status: Pending
```

### payment_intent.payment_failed
```csharp
Payment:
  - Status: Failed
  - FailureReason: Error message from Stripe
  - ProcessedAt: Current UTC time
  - PaymentGatewayResponse: Full Stripe response

Order (if status is PendingPayment):
  - Status: Cancelled
```

### charge.refunded
```csharp
Payment:
  - Status: Refunded or PartiallyRefunded
  - RefundedAmount: Amount refunded in cents
  - RefundedAt: Current UTC time
  - PaymentGatewayResponse: Full Stripe response
```

## Logging
All webhook events are logged with:
- Event type and ID
- Payment Intent ID or Charge ID
- Processing results
- Any errors or warnings

Example log:
```
[Information] Processing Stripe webhook event
[Information] Webhook event type: payment_intent.succeeded, ID: evt_1234
[Information] Processing payment_intent.succeeded: PaymentIntentId=pi_xxx, Amount=5000
[Information] Payment updated to Succeeded: PaymentId=1, OrderId=123
[Information] Order status updated to Pending: OrderId=123
```

## Idempotency
Safe to process the same event multiple times:
- Checks current payment status before updating
- Returns early if already in target state
- No duplicate charges or status changes

## Stripe Dashboard Setup
1. Go to: https://dashboard.stripe.com/webhooks
2. Click: "Add endpoint"
3. URL: `https://yourdomain.com/api/webhook/stripe`
4. Events: Select `payment_intent.succeeded`, `payment_intent.payment_failed`, `charge.refunded`
5. Copy webhook signing secret (starts with `whsec_`)
6. Add to configuration

## Health Check
Monitor these metrics:
- Webhook delivery success rate in Stripe Dashboard
- Application logs for webhook processing
- Database: Payment and Order status updates
- Alert on repeated 500 errors

---
**Endpoint:** `/api/webhook/stripe`  
**Method:** POST  
**Auth:** None (Stripe signature)  
**Status:** ? Production Ready
