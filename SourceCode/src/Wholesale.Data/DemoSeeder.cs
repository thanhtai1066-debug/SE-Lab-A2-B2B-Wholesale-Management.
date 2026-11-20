using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Wholesale.Data;

public static class DemoSeeder
{
    public static async Task SeedAsync(WholesaleDbContext db, CancellationToken ct = default)
    {
        if (!await db.Items.AnyAsync(ct))
        {
            db.Items.AddRange(new[]
            {
                new Item { ItemName = "A4 Copy Paper", Size = "Ream 500", Price = 78000m, StockQuantity = 120 },
                new Item { ItemName = "Blue Ballpoint Pen", Size = "Box 50", Price = 155000m, StockQuantity = 80 },
                new Item { ItemName = "A5 Notebook", Size = "Pack 12", Price = 132000m, StockQuantity = 55 },
                new Item { ItemName = "Shipping Box Small", Size = "Pack 20", Price = 110000m, StockQuantity = 68 },
                new Item { ItemName = "Shipping Box Medium", Size = "Pack 20", Price = 165000m, StockQuantity = 48 },
                new Item { ItemName = "Packing Tape", Size = "Roll 100m", Price = 26000m, StockQuantity = 95 },
                new Item { ItemName = "USB 32GB", Size = "Piece", Price = 99000m, StockQuantity = 34 },
                new Item { ItemName = "Wireless Mouse", Size = "Piece", Price = 179000m, StockQuantity = 24 },
                new Item { ItemName = "Office Keyboard", Size = "Piece", Price = 219000m, StockQuantity = 19 },
                new Item { ItemName = "A4 File Folder", Size = "Pack 25", Price = 72000m, StockQuantity = 100 },
                new Item { ItemName = "Whiteboard Marker", Size = "Box 12", Price = 125000m, StockQuantity = 72 },
                new Item { ItemName = "Receipt Paper", Size = "Box 20", Price = 288000m, StockQuantity = 18 },
                new Item { ItemName = "Bubble Wrap", Size = "Roll 50m", Price = 175000m, StockQuantity = 7 },
                new Item { ItemName = "Bubble Mailer", Size = "Pack 100", Price = 260000m, StockQuantity = 15 },
                new Item { ItemName = "Safety Gloves", Size = "Box 100", Price = 98000m, StockQuantity = 5 },
                new Item { ItemName = "Shipping Label", Size = "Roll 500", Price = 138000m, StockQuantity = 62 },
                new Item { ItemName = "Heavy Duty Stapler", Size = "Piece", Price = 249000m, StockQuantity = 12 },
                new Item { ItemName = "Black Printer Toner", Size = "Cartridge", Price = 425000m, StockQuantity = 9 },
                new Item { ItemName = "Desk Organizer", Size = "Piece", Price = 92000m, StockQuantity = 33 },
                new Item { ItemName = "Microfiber Cloth", Size = "Pack 20", Price = 85000m, StockQuantity = 44 }
            });
            await db.SaveChangesAsync(ct);
        }
        if (!await db.Agents.AnyAsync(ct))
        {
            db.Agents.AddRange(new[]
            {
                new Agent { AgentName = "Wholesale Partner 01", Address = "11 Demo Street, District 2, Ho Chi Minh City", Phone = "0901000001" },
                new Agent { AgentName = "Wholesale Partner 02", Address = "12 Demo Street, District 3, Ho Chi Minh City", Phone = "0901000002" },
                new Agent { AgentName = "Wholesale Partner 03", Address = "13 Demo Street, District 4, Ho Chi Minh City", Phone = "0901000003" },
                new Agent { AgentName = "Wholesale Partner 04", Address = "14 Demo Street, District 5, Ho Chi Minh City", Phone = "0901000004" },
                new Agent { AgentName = "Wholesale Partner 05", Address = "15 Demo Street, District 6, Ho Chi Minh City", Phone = "0901000005" },
                new Agent { AgentName = "Wholesale Partner 06", Address = "16 Demo Street, District 7, Ho Chi Minh City", Phone = "0901000006" },
                new Agent { AgentName = "Wholesale Partner 07", Address = "17 Demo Street, District 8, Ho Chi Minh City", Phone = "0901000007" },
                new Agent { AgentName = "Wholesale Partner 08", Address = "18 Demo Street, District 9, Ho Chi Minh City", Phone = "0901000008" },
                new Agent { AgentName = "Wholesale Partner 09", Address = "19 Demo Street, District 10, Ho Chi Minh City", Phone = "0901000009" },
                new Agent { AgentName = "Wholesale Partner 10", Address = "20 Demo Street, District 1, Ho Chi Minh City", Phone = "0901000010" },
                new Agent { AgentName = "Wholesale Partner 11", Address = "21 Demo Street, District 2, Ho Chi Minh City", Phone = "0901000011" },
                new Agent { AgentName = "Wholesale Partner 12", Address = "22 Demo Street, District 3, Ho Chi Minh City", Phone = "0901000012" },
                new Agent { AgentName = "Wholesale Partner 13", Address = "23 Demo Street, District 4, Ho Chi Minh City", Phone = "0901000013" },
                new Agent { AgentName = "Wholesale Partner 14", Address = "24 Demo Street, District 5, Ho Chi Minh City", Phone = "0901000014" },
                new Agent { AgentName = "Wholesale Partner 15", Address = "25 Demo Street, District 6, Ho Chi Minh City", Phone = "0901000015" },
                new Agent { AgentName = "Wholesale Partner 16", Address = "26 Demo Street, District 7, Ho Chi Minh City", Phone = "0901000016" },
                new Agent { AgentName = "Wholesale Partner 17", Address = "27 Demo Street, District 8, Ho Chi Minh City", Phone = "0901000017" },
                new Agent { AgentName = "Wholesale Partner 18", Address = "28 Demo Street, District 9, Ho Chi Minh City", Phone = "0901000018" },
                new Agent { AgentName = "Wholesale Partner 19", Address = "29 Demo Street, District 10, Ho Chi Minh City", Phone = "0901000019" },
                new Agent { AgentName = "Wholesale Partner 20", Address = "30 Demo Street, District 1, Ho Chi Minh City", Phone = "0901000020" }
            });
            await db.SaveChangesAsync(ct);
        }
        if (!await db.Users.AnyAsync(ct))
        {
            var agents = await db.Agents.OrderBy(x => x.AgentId).Take(18).ToListAsync(ct);
            var hasher = new PasswordHasher<UserAccount>();
            var users = new List<UserAccount>
            {
                new UserAccount { UserName = "admin", Email = "admin@example.test", Role = "Admin" },
                new UserAccount { UserName = "admin02", Email = "admin02@example.test", Role = "Admin" }
            };
            for (var i = 0; i < agents.Count; i++)
                users.Add(new UserAccount { UserName = $"agent{i+1:00}", Email = $"agent{i+1:00}@example.test", Role = "Agent", AgentId = agents[i].AgentId });
            foreach (var user in users)
                user.PasswordHash = hasher.HashPassword(user, user.Role == "Admin" ? "Admin@12345!" : "Agent@12345!");
            db.Users.AddRange(users);
            await db.SaveChangesAsync(ct);
        }
        if (!await db.Orders.AnyAsync(ct))
        {
            var agents = await db.Agents.OrderBy(x => x.AgentId).Take(18).ToArrayAsync(ct);
            var items = await db.Items.OrderBy(x => x.ItemId).ToArrayAsync(ct);
            var data = new (int AgentIndex, string Status)[]
            {
                (0, "Processing"),
                (1, "Shipped"),
                (2, "Shipped"),
                (3, "Cancelled"),
                (4, "Pending"),
                (5, "Processing"),
                (6, "Shipped"),
                (7, "Shipped"),
                (8, "Cancelled"),
                (9, "Pending"),
                (10, "Processing"),
                (11, "Shipped"),
                (12, "Shipped"),
                (13, "Cancelled"),
                (14, "Pending"),
                (15, "Processing"),
                (16, "Shipped"),
                (17, "Shipped"),
                (0, "Cancelled"),
                (1, "Pending")
            };
            var orderEntities = new List<SalesOrder>();
            for (var i = 0; i < data.Length; i++)
            {
                var order = new SalesOrder { AgentId = agents[data[i].AgentIndex].AgentId,
                    OrderDate = DateTime.UtcNow.AddDays(-(i + 1)), OrderStatus = data[i].Status };
                orderEntities.Add(order);
                db.Orders.Add(order);
            }
            await db.SaveChangesAsync(ct);
            var details = new (int OrderIndex, int ItemIndex, int Quantity)[]
            {
                (0, 2, 2),
                (1, 5, 3),
                (2, 8, 4),
                (3, 11, 5),
                (4, 14, 6),
                (5, 17, 1),
                (6, 0, 2),
                (7, 3, 3),
                (8, 6, 4),
                (9, 9, 5),
                (10, 12, 6),
                (11, 15, 1),
                (12, 18, 2),
                (13, 1, 3),
                (14, 4, 4),
                (15, 7, 5),
                (16, 10, 6),
                (17, 13, 1),
                (18, 16, 2),
                (19, 19, 3),
                (0, 11, 2),
                (1, 18, 3),
                (2, 5, 1),
                (3, 12, 2),
                (4, 19, 3),
                (5, 6, 1),
                (6, 13, 2),
                (7, 0, 3)
            };
            foreach (var line in details)
            {
                var item = items[line.ItemIndex];
                db.OrderDetails.Add(new OrderDetail { OrderId = orderEntities[line.OrderIndex].OrderId,
                    ItemId = item.ItemId, Quantity = line.Quantity, UnitAmount = item.Price });
            }
            // Demo stock represents CURRENT availability after these historical sample orders.
            await db.SaveChangesAsync(ct);
        }
    }
}
