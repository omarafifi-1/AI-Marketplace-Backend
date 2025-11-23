using AI_Marketplace.Application.Vendors.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Commands
{
    public  class EditVendorProfileCommand : IRequest<VendorProfileDto>
    {
        public int UserId { get; }
        public VendorEditDto VendorEditDto { get; }
        public EditVendorProfileCommand(int userId, VendorEditDto vendorEditDto)
        {
            UserId = userId;
            VendorEditDto = vendorEditDto;
        }
    }
}
