using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Payment.DTOs
{
    public record RefundPaymentDto(int PaymentId,long RefundAmount, string? RefundReason);
}
