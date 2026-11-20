using Microsoft.EntityFrameworkCore;

namespace Wholesale.Data;

public class WholesaleDbContext(DbContextOptions<WholesaleDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<SalesOrder> Orders => Set<SalesOrder>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<UserAccount> Users => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Item>(e =>
        {
            e.ToTable("Item", t => { t.HasCheckConstraint("CK_Item_Price", "[Price] >= 0"); t.HasCheckConstraint("CK_Item_Stock", "[StockQuantity] >= 0"); });
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemName).HasMaxLength(120).IsRequired();
            e.Property(x => x.Size).HasMaxLength(30).IsRequired();
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasIndex(x => new { x.ItemName, x.Size }).IsUnique();
        });
        m.Entity<Agent>(e =>
        {
            e.ToTable("Agent"); e.HasKey(x => x.AgentId);
            e.Property(x => x.AgentName).HasMaxLength(120).IsRequired();
            e.Property(x => x.Address).HasMaxLength(240).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(25).IsRequired();
        });
        m.Entity<SalesOrder>(e =>
        {
            e.ToTable("Order", t => t.HasCheckConstraint("CK_Order_Status", "[OrderStatus] IN ('Pending','Processing','Shipped','Cancelled')"));
            e.HasKey(x => x.OrderId);
            e.Property(x => x.OrderDate).HasDefaultValueSql(Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite" ? "CURRENT_TIMESTAMP" : "GETUTCDATE()");
            e.Property(x => x.OrderStatus).HasMaxLength(20).HasDefaultValue("Pending").IsRequired();
            e.HasOne(x => x.Agent).WithMany(x => x.Orders).HasForeignKey(x => x.AgentId).OnDelete(DeleteBehavior.Restrict);
        });
        m.Entity<OrderDetail>(e =>
        {
            e.ToTable("OrderDetail", t => { t.HasCheckConstraint("CK_Detail_Qty", "[Quantity] > 0"); t.HasCheckConstraint("CK_Detail_Price", "[UnitAmount] >= 0"); });
            e.HasKey(x => x.Id);
            e.Property(x => x.UnitAmount).HasPrecision(18, 2);
            e.HasIndex(x => new { x.OrderId, x.ItemId }).IsUnique();
            e.HasOne(x => x.Order).WithMany(x => x.Details).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Item).WithMany(x => x.Details).HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        });
        m.Entity<UserAccount>(e =>
        {
            e.ToTable("Users"); e.HasKey(x => x.UserId);
            e.Property(x => x.UserName).HasMaxLength(60).IsRequired();
            e.Property(x => x.Email).HasMaxLength(160).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            e.Property(x => x.Role).HasMaxLength(10).IsRequired();
            e.HasIndex(x => x.UserName).IsUnique(); e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.Agent).WithMany(x => x.Accounts).HasForeignKey(x => x.AgentId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
