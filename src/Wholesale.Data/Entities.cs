namespace Wholesale.Data;

public class Item
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = "";
    public string Size { get; set; } = "";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
}

public class Agent
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public ICollection<SalesOrder> Orders { get; set; } = new List<SalesOrder>();
    public ICollection<UserAccount> Accounts { get; set; } = new List<UserAccount>();
}

public class SalesOrder
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public int AgentId { get; set; }
    public Agent Agent { get; set; } = null!;
    public string OrderStatus { get; set; } = "Pending";
    public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
}

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public SalesOrder Order { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitAmount { get; set; }
}

public class UserAccount
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool IsLocked { get; set; }
    public int FailedLoginCount { get; set; }
    public string Role { get; set; } = "Agent";
    public int? AgentId { get; set; }
    public Agent? Agent { get; set; }
}
