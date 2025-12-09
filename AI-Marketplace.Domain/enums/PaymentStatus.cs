using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Domain.enums
{
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Succeeded,
        Failed,
        Cancelled,
        Refunded,
        PartiallyRefunded
    }
}
