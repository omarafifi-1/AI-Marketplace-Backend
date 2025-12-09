using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address> AddAddressAsync(Address address);
        Task<Address?> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(int id);
        Task<Address?> GetAddressByIdAsync(int id);
        Task<IEnumerable<Address>> GetAddressesByUserIdAsync(int userId);
    }
}
