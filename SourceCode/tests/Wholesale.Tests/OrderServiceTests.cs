using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Wholesale.Core;
using Wholesale.Data;
using Xunit;

namespace Wholesale.Tests;
public class OrderServiceTests
{
    private static async Task<(SqliteConnection connection, WholesaleDbContext db)> MakeDbAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var opts = new DbContextOptionsBuilder<WholesaleDbContext>().UseSqlite(connection).Options;
        var db = new WholesaleDbContext(opts);
        await db.Database.EnsureCreatedAsync();
        db.Agents.AddRange(new Agent { AgentName = "Agent One", Address = "Local street", Phone = "111" },
            new Agent { AgentName = "Agent Two", Address = "Second street", Phone = "222" });
        db.Items.AddRange(new Item { ItemName = "Pens", Size = "Box", Price = 20000m, StockQuantity = 10 },
            new Item { ItemName = "Paper", Size = "Ream", Price = 50000m, StockQuantity = 1 });
        await db.SaveChangesAsync();
        return (connection, db);
    }

    [Fact] public async Task SuccessfulOrderCreatesMasterDetailsAndDeductsStock()
    {
        var (conn, db) = await MakeDbAsync();
        await using (conn) await using (db)
        {
            int orderId = await new OrderService(db).PlaceOrderAsync(1, new[] { new OrderLineRequest(1, 3) });
            Assert.True(orderId > 0);
            Assert.Equal(1, await db.Orders.CountAsync());
            Assert.Equal(1, await db.OrderDetails.CountAsync());
            Assert.Equal(7, (await db.Items.AsNoTracking().SingleAsync(i => i.ItemId == 1)).StockQuantity);
            Assert.Equal(20000m, (await db.OrderDetails.SingleAsync()).UnitAmount);
        }
    }

    [Fact] public async Task InsufficientStockDoesNotPersistOrder()
    {
        var (conn, db) = await MakeDbAsync();
        await using (conn) await using (db)
        {
            await Assert.ThrowsAsync<OrderRejectedException>(() => new OrderService(db).PlaceOrderAsync(1,
                new[] { new OrderLineRequest(2, 10) }));
            Assert.Equal(0, await db.Orders.CountAsync());
            Assert.Equal(0, await db.OrderDetails.CountAsync());
            Assert.Equal(1, (await db.Items.AsNoTracking().SingleAsync(i => i.ItemId == 2)).StockQuantity);
        }
    }

    [Fact] public async Task FailureOnSecondLineRollsBackFirstStockDeduction()
    {
        var (conn, db) = await MakeDbAsync();
        await using (conn) await using (db)
        {
            await Assert.ThrowsAsync<OrderRejectedException>(() => new OrderService(db).PlaceOrderAsync(1,
                new[] { new OrderLineRequest(1, 3), new OrderLineRequest(2, 10) }));
            Assert.Equal(0, await db.Orders.CountAsync());
            Assert.Equal(0, await db.OrderDetails.CountAsync());
            Assert.Equal(10, (await db.Items.AsNoTracking().SingleAsync(i => i.ItemId == 1)).StockQuantity);
            Assert.Equal(1, (await db.Items.AsNoTracking().SingleAsync(i => i.ItemId == 2)).StockQuantity);
        }
    }

    [Fact] public async Task AgentsCannotReadOneAnothersOrders()
    {
        var (conn, db) = await MakeDbAsync();
        await using (conn) await using (db)
        {
            var service = new OrderService(db);
            int orderId = await service.PlaceOrderAsync(1, new[] { new OrderLineRequest(1, 1) });
            Assert.NotNull(await service.MyOrderAsync(orderId, 1));
            Assert.Null(await service.MyOrderAsync(orderId, 2));
            Assert.Empty(await service.MyOrdersAsync(2));
        }
    }
}
