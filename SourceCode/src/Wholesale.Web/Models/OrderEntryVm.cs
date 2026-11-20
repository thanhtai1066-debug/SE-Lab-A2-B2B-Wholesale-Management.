namespace Wholesale.Web.Models;
public sealed class OrderEntryVm
{
    public List<OrderItemVm> Lines { get; set; } = new();
}
public sealed class OrderItemVm
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = "";
    public string Size { get; set; } = "";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Quantity { get; set; }
}
