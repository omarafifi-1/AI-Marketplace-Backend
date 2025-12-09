using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Infrastructure.ExternalServices.payment
{
    public class StripeOptions
    {
        public const string SectionName = "Stripe"; 
        public string SecretKey { get; set; }        
        public string PublishableKey { get; set; }
        public string WebhookSecret { get; set; }
    }
}
