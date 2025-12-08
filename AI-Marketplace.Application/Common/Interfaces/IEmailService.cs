using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}
