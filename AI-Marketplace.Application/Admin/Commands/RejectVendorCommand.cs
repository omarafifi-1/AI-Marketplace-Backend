using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Commands
{
    public class RejectVendorCommand : IRequest<String>
    {
        public int StoreId { get; set; }
    }
}
