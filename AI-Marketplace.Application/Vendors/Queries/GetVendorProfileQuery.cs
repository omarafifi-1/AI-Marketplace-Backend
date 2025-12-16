using AI_Marketplace.Application.Vendors.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Queries
{
    public class GetVendorProfileQuery : IRequest<VendorProfileDto>
    {
        public int UserId { get; set; }
        public GetVendorProfileQuery(int userId)
        {
            UserId = userId;
        }
    }
}
