using AI_Marketplace.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Queries
{
    public class GetVendorOrdersQuery : IRequest<List<OrderDto>>
    {
        public int VendorId { get; }
        public GetVendorOrdersQuery(int vendorId)
        {
            VendorId = vendorId;
        }
    }
}
