using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Commands
{
    public class ApproveVendorCommand : IRequest<String>
    {
        public int StoreId { get; set; }
        public int AdminId { get; set; }
    }
}
