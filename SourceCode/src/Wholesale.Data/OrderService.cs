using Microsoft.EntityFrameworkCore;
using Wholesale.Core;

namespace Wholesale.Data;

public class OrderService(WholesaleDbContext db)
{
    public async Task<int> PlaceOrderAsync(int agentId, IEnumerable<OrderLineRequest> requested, CancellationToken ct = default)
    {
        var lines = OrderRules.ValidateLines(requested);
        if (!await db.Agents.AnyAsync(x => x.AgentId == agentId, ct))
            throw new OrderRejectedException("Agent account was not found.");

        // A single transaction encompasses the master row, stock updates and detail rows.
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var order = new SalesOrder { AgentId = agentId, OrderStatus = "Pending", OrderDate = DateTime.UtcNow };
            db.Orders.Add(order);
            await db.SaveChangesAsync(ct);

            // Update each stock quantity atomically. This prevents two simultaneous orders
            // from both consuming the same remaining units.
            foreach (var line in lines.OrderBy(x => x.ItemId))
            {
                var item = await db.Items.AsNoTracking()
                    .Where(x => x.ItemId == line.ItemId)
                    .Select(x => new { x.ItemId, x.Price }).SingleOrDefaultAsync(ct);
                if (item == null) throw new OrderRejectedException($"Item #{line.ItemId} no longer exists.");
                var affected = await db.Items.Where(x => x.ItemId == line.ItemId && x.StockQuantity >= line.Quantity)
                    .ExecuteUpdateAsync(update => update.SetProperty(x => x.StockQuantity, x => x.StockQuantity - line.Quantity), ct);
                if (affected != 1) throw new OrderRejectedException($"Not enough stock for item #{line.ItemId}; the entire order was cancelled.");
                db.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId, ItemId = line.ItemId,
                    Quantity = line.Quantity, UnitAmount = item.Price
                });
            }
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return order.OrderId;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            db.ChangeTracker.Clear();
            throw;
        }
    }

    public Task<List<SalesOrder>> MyOrdersAsync(int agentId, CancellationToken ct = default) =>
        db.Orders.AsNoTracking().Where(o => o.AgentId == agentId)
            .Include(o => o.Details).OrderByDescending(o => o.OrderDate).ToListAsync(ct);

    public Task<SalesOrder?> MyOrderAsync(int orderId, int agentId, CancellationToken ct = default) =>
        db.Orders.AsNoTracking().Include(o => o.Agent).Include(o => o.Details).ThenInclude(d => d.Item)
            .SingleOrDefaultAsync(o => o.OrderId == orderId && o.AgentId == agentId, ct);
}
