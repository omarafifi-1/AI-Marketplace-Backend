using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Repositories.Orders
{
    public class MasterOrderRepository : IMasterOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public MasterOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MasterOrder> CreateAsync(MasterOrder masterOrder, CancellationToken cancellationToken = default)
        {
            _context.MasterOrders.Add(masterOrder);
            await _context.SaveChangesAsync(cancellationToken);
            return masterOrder;
        }

        public async Task<MasterOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.MasterOrders
                .Include(mo => mo.Buyer)
                .Include(mo => mo.ShippingAddressEntity)
                .FirstOrDefaultAsync(mo => mo.Id == id, cancellationToken);
        }

        public async Task<MasterOrder?> GetByIdWithChildOrdersAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.MasterOrders
                .Include(mo => mo.Buyer)
                .Include(mo => mo.ShippingAddressEntity)
                .Include(mo => mo.ChildOrders)
                    .ThenInclude(o => o.Store)
                .Include(mo => mo.ChildOrders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(mo => mo.Payments)
                .FirstOrDefaultAsync(mo => mo.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(MasterOrder masterOrder, CancellationToken cancellationToken = default)
        {
            _context.MasterOrders.Update(masterOrder);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
