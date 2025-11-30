using AI_Marketplace.Application.Vendors.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Queries
{
    public class GetVendorStatisticsQuery : IRequest<VendorStatisticsDto>
    {
        public int VendorId { get; }
        public GetVendorStatisticsQuery(int vendorId)
        {
            VendorId = vendorId;
        }
    }
}
