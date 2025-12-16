using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Payment.Commands
{
    public class CreatePaymentIntentCommand : IRequest<string>, IBaseRequest
    {
        public int MasterOrderId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
    }
}
