using Wholesale.Core;
using Xunit;

namespace Wholesale.Tests;
public class OrderRulesTests
{
    [Fact] public void RejectsEmptyOrder() => Assert.Throws<OrderRejectedException>(() => OrderRules.ValidateLines(Array.Empty<OrderLineRequest>()));
    [Fact] public void RejectsDuplicateProducts() => Assert.Throws<OrderRejectedException>(() =>
        OrderRules.ValidateLines(new[] { new OrderLineRequest(1, 2), new OrderLineRequest(1, 3) }));
    [Theory]
    [InlineData(0)] [InlineData(-1)]
    public void RejectsNonPositiveQuantities(int qty) => Assert.Throws<OrderRejectedException>(() =>
        OrderRules.ValidateLines(new[] { new OrderLineRequest(1, qty) }));
    [Fact] public void StockCannotBeOversold() => Assert.Throws<OrderRejectedException>(() => OrderRules.DeductStock(4, 5));
    [Fact] public void DeductsTheExactQuantity() => Assert.Equal(7, OrderRules.DeductStock(10, 3));
    [Fact] public void LineTotalCalculatesMoneyCorrectly() => Assert.Equal(61500m, OrderRules.LineTotal(3, 20500m));
    [Fact] public void OrderTotalCombinesMultipleLines() => Assert.Equal(94000m,
        OrderRules.OrderTotal(new[] { (2, 32000m), (3, 10000m) }));
    [Fact] public void LockoutBeginsAtFifthWrongPassword()
    {
        Assert.False(OrderRules.MustLock(4));
        Assert.True(OrderRules.MustLock(5));
        Assert.True(OrderRules.MustLock(6));
    }
}
