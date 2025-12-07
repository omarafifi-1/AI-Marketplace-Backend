using AI_Marketplace.Application.Vendors.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Queries
{
    public class GetAllVendorsQuery : IRequest<List<VendorProfileDto>>
    {
    }
}
