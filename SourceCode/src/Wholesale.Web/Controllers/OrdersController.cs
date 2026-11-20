using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wholesale.Core;
using Wholesale.Data;
using Wholesale.Web.Models;

namespace Wholesale.Web.Controllers;

[Authorize(Roles = "Agent")]
public class OrdersController(WholesaleDbContext db, OrderService orderService) : Controller
{
    private int AgentId => int.Parse(User.FindFirstValue("AgentId")!);

    private async Task<OrderEntryVm> LoadItemsAsync() => new()
    {
        Lines = await db.Items.AsNoTracking().OrderBy(x => x.ItemName)
            .Select(x => new OrderItemVm { ItemId = x.ItemId, ItemName = x.ItemName,
                Size = x.Size, Price = x.Price, StockQuantity = x.StockQuantity })
            .ToListAsync()
    };

    [HttpGet]
    public async Task<IActionResult> Create() => View(await LoadItemsAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderEntryVm model)
    {
        try
        {
            // Item names, prices and the agent identifier are NEVER trusted from posted form fields.
            var requested = (model.Lines ?? new()).Where(x => x.Quantity != 0)
                .Select(x => new OrderLineRequest(x.ItemId, x.Quantity));
            var orderId = await orderService.PlaceOrderAsync(AgentId, requested);
            TempData["Success"] = $"Order #{orderId} placed. Stock has been updated.";
            return RedirectToAction("Invoice", new { id = orderId });
        }
        catch (OrderRejectedException ex) { ModelState.AddModelError("", ex.Message); }
        catch (DbUpdateException) { ModelState.AddModelError("", "Order could not be completed. No partial order was saved."); }
        var items = await LoadItemsAsync();
        var choices = (model.Lines ?? new()).GroupBy(x => x.ItemId).ToDictionary(g => g.Key, g => g.First().Quantity);
        foreach (var row in items.Lines)
            if (choices.TryGetValue(row.ItemId, out var qty)) row.Quantity = qty;
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Mine() => View(await orderService.MyOrdersAsync(AgentId));

    [HttpGet]
    public async Task<IActionResult> Invoice(int id)
    {
        var order = await orderService.MyOrderAsync(id, AgentId);
        return order is null ? NotFound() : View(order);
    }
}
