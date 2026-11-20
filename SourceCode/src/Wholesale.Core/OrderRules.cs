namespace Wholesale.Core;

public sealed record OrderLineRequest(int ItemId, int Quantity);
public sealed class OrderRejectedException(string message) : Exception(message);

public static class OrderRules
{
    public static IReadOnlyList<OrderLineRequest> ValidateLines(IEnumerable<OrderLineRequest>? lines)
    {
        if (lines == null) throw new OrderRejectedException("No items were submitted.");
        var list = lines.ToList();
        if (list.Count == 0) throw new OrderRejectedException("Select at least one item.");
        if (list.Count > 100) throw new OrderRejectedException("An order cannot exceed 100 distinct items.");
        if (list.Any(x => x.ItemId <= 0 || x.Quantity <= 0))
            throw new OrderRejectedException("Every order line requires a valid item and a positive quantity.");
        if (list.Select(x => x.ItemId).Distinct().Count() != list.Count)
            throw new OrderRejectedException("An item may appear only once in an order.");
        return list;
    }

    public static int DeductStock(int current, int quantity)
    {
        if (current < 0 || quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > current) throw new OrderRejectedException("Insufficient stock.");
        return current - quantity;
    }

    public static decimal LineTotal(int quantity, decimal price)
    {
        if (quantity <= 0 || price < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        return checked(quantity * price);
    }

    public static decimal OrderTotal(IEnumerable<(int Quantity, decimal UnitPrice)> lines) =>
        lines.Sum(x => LineTotal(x.Quantity, x.UnitPrice));

    public static bool MustLock(int failedAttempts, int maximumAttempts = 5) =>
        maximumAttempts > 0 && failedAttempts >= maximumAttempts;
}
