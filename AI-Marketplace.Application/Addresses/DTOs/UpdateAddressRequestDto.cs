using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.DTOs
{
    public class UpdateAddressRequestDto
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
