using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Domain.enums
{
    public enum PaymentMethod
    {
        Stripe,
        PayPal,
        CreditCard,
        DebitCard,
        BankTransfer,
        Wallet,
        CashOnDelivery
    }
}
