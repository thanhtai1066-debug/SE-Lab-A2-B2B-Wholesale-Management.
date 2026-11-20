using Microsoft.EntityFrameworkCore;
using Wholesale.Data;

namespace Wholesale.Desktop;
public sealed class MainForm : Form
{
    private readonly IDbContextFactory<WholesaleDbContext> factory;
    private readonly UserAccount admin;
    private readonly TabControl tabs = new() { Dock = DockStyle.Fill };
    private readonly DataGridView itemsGrid = Grid();
    private readonly DataGridView agentsGrid = Grid();
    private readonly DataGridView ordersGrid = Grid();
    private readonly DataGridView bestSellersGrid = Grid();
    private readonly DataGridView lowStockGrid = Grid();
    private readonly DataGridView revenueGrid = Grid();
    private readonly DataGridView manifestGrid = Grid();
    private readonly TextBox itemName = T(195), itemSize = T(105);
    private readonly NumericUpDown itemPrice = Num(1000000000), itemStock = Num(100000);
    private readonly TextBox agentName = T(175), agentAddress = T(245), agentPhone = T(130);
    private readonly ComboBox statusPicker = new() { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown lowStockThreshold = Num(1000);
    private int currentItemId, currentAgentId;

    private static DataGridView Grid() => new()
    {
        Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
        BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false
    };
    private static TextBox T(int width) => new() { Width = width };
    private static NumericUpDown Num(decimal max) => new() { Maximum = max, Width = 115, ThousandsSeparator = true };
    private static Label L(string s) => new() { Text = s, AutoSize = true, Padding = new Padding(4, 7, 0, 0) };
    private static Button B(string s, EventHandler callback)
    {
        var button = new Button { Text = s, AutoSize = true, Height = 33, Margin = new Padding(4) };
        button.Click += callback;
        return button;
    }
    private static FlowLayoutPanel Bar() => new()
    {
        Dock = DockStyle.Top, Height = 82, AutoScroll = true, WrapContents = true,
        Padding = new Padding(5, 10, 0, 5), BackColor = Color.FromArgb(234, 241, 249)
    };
    private static bool Confirm(string text) => MessageBox.Show(text, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    private static void Alert(Exception ex) => MessageBox.Show(ex.GetBaseException().Message,
        "Unable to complete operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    private async Task UseDb(Func<WholesaleDbContext, Task> action)
    {
        try { await using var db = await factory.CreateDbContextAsync(); await action(db); }
        catch (Exception ex) { Alert(ex); }
    }

    public MainForm(IDbContextFactory<WholesaleDbContext> factory, UserAccount admin)
    {
        this.factory = factory; this.admin = admin;
        Text = $"Wholesale Back-Office | Signed in: {admin.UserName}";
        WindowState = FormWindowState.Maximized; MinimumSize = new Size(1024, 680);
        Font = new Font("Segoe UI", 10);
        statusPicker.Items.AddRange(new object[] { "Pending", "Processing", "Shipped", "Cancelled" });
        statusPicker.SelectedIndex = 0; lowStockThreshold.Value = 10;
        tabs.TabPages.Add(BuildItemsTab());
        tabs.TabPages.Add(BuildAgentsTab());
        tabs.TabPages.Add(BuildOrdersTab());
        tabs.TabPages.Add(BuildManifestTab());
        tabs.TabPages.Add(BuildDashboardTab());
        Controls.Add(tabs);
        Shown += async (_, _) => { await RefreshItemsAsync(); await RefreshAgentsAsync(); await RefreshOrdersAsync(); await RefreshDashboardAsync(); await RefreshManifestAsync(); };
    }

    private TabPage BuildItemsTab()
    {
        var page = new TabPage("Items & Stock"); var bar = Bar();
        bar.Controls.AddRange(new Control[] { L("Name"), itemName, L("Size"), itemSize, L("Price VND"), itemPrice,
            L("Stock"), itemStock, B("Save item", async (_, _) => await SaveItemAsync()),
            B("Clear", (_, _) => ClearItem()), B("Delete", async (_, _) => await DeleteItemAsync()),
            B("Reload", async (_, _) => await RefreshItemsAsync()) });
        page.Controls.Add(itemsGrid); page.Controls.Add(bar);
        itemsGrid.SelectionChanged += (_, _) =>
        {
            if (itemsGrid.SelectedRows.Count == 0) return;
            var c = itemsGrid.SelectedRows[0].Cells;
            currentItemId = Convert.ToInt32(c["ItemId"].Value);
            itemName.Text = c["ItemName"].Value?.ToString(); itemSize.Text = c["Size"].Value?.ToString();
            itemPrice.Value = Math.Min(itemPrice.Maximum, Convert.ToDecimal(c["Price"].Value));
            itemStock.Value = Math.Min(itemStock.Maximum, Convert.ToDecimal(c["StockQuantity"].Value));
        };
        return page;
    }
    private void ClearItem() { itemsGrid.CurrentCell = null; itemsGrid.ClearSelection(); currentItemId = 0; itemName.Clear(); itemSize.Clear(); itemPrice.Value = 0; itemStock.Value = 0; }
    private async Task RefreshItemsAsync() => await UseDb(async db =>
    {
        itemsGrid.DataSource = await db.Items.AsNoTracking().OrderBy(i => i.ItemId)
            .Select(i => new { i.ItemId, i.ItemName, i.Size, i.Price, i.StockQuantity }).ToListAsync();
    });
    private async Task SaveItemAsync()
    {
        if (string.IsNullOrWhiteSpace(itemName.Text) || string.IsNullOrWhiteSpace(itemSize.Text))
        { MessageBox.Show("Name and size are required."); return; }
        await UseDb(async db =>
        {
            var entity = currentItemId == 0 ? new Item() : await db.Items.FindAsync(currentItemId) ?? throw new InvalidOperationException("Item missing.");
            entity.ItemName = itemName.Text.Trim(); entity.Size = itemSize.Text.Trim();
            entity.Price = itemPrice.Value; entity.StockQuantity = (int)itemStock.Value;
            if (currentItemId == 0) db.Items.Add(entity);
            await db.SaveChangesAsync();
            ClearItem(); await RefreshItemsAsync();
        });
    }
    private async Task DeleteItemAsync()
    {
        if (currentItemId == 0 || !Confirm("Remove the selected item? Items in existing orders are protected.")) return;
        await UseDb(async db =>
        {
            var item = await db.Items.FindAsync(currentItemId);
            if (item != null) { db.Items.Remove(item); await db.SaveChangesAsync(); }
            ClearItem(); await RefreshItemsAsync();
        });
    }

    private TabPage BuildAgentsTab()
    {
        var page = new TabPage("Agents"); var bar = Bar();
        bar.Controls.AddRange(new Control[] { L("Name"), agentName, L("Address"), agentAddress,
            L("Phone"), agentPhone, B("Save agent", async (_, _) => await SaveAgentAsync()),
            B("Clear", (_, _) => ClearAgent()), B("Delete", async (_, _) => await DeleteAgentAsync()),
            B("Reload", async (_, _) => await RefreshAgentsAsync()) });
        page.Controls.Add(agentsGrid); page.Controls.Add(bar);
        agentsGrid.SelectionChanged += (_, _) =>
        {
            if (agentsGrid.SelectedRows.Count == 0) return;
            var c = agentsGrid.SelectedRows[0].Cells;
            currentAgentId = Convert.ToInt32(c["AgentId"].Value);
            agentName.Text = c["AgentName"].Value?.ToString();
            agentAddress.Text = c["Address"].Value?.ToString();
            agentPhone.Text = c["Phone"].Value?.ToString();
        };
        return page;
    }
    private void ClearAgent() { agentsGrid.CurrentCell = null; agentsGrid.ClearSelection(); currentAgentId = 0; agentName.Clear(); agentAddress.Clear(); agentPhone.Clear(); }
    private async Task RefreshAgentsAsync() => await UseDb(async db =>
    {
        agentsGrid.DataSource = await db.Agents.AsNoTracking().OrderBy(a => a.AgentId)
            .Select(a => new { a.AgentId, a.AgentName, a.Address, a.Phone }).ToListAsync();
    });
    private async Task SaveAgentAsync()
    {
        if (string.IsNullOrWhiteSpace(agentName.Text) || string.IsNullOrWhiteSpace(agentAddress.Text) || string.IsNullOrWhiteSpace(agentPhone.Text))
        { MessageBox.Show("Complete all agent fields."); return; }
        await UseDb(async db =>
        {
            var entity = currentAgentId == 0 ? new Agent() : await db.Agents.FindAsync(currentAgentId) ?? throw new InvalidOperationException("Agent missing.");
            entity.AgentName = agentName.Text.Trim(); entity.Address = agentAddress.Text.Trim(); entity.Phone = agentPhone.Text.Trim();
            if (currentAgentId == 0) db.Agents.Add(entity);
            await db.SaveChangesAsync(); ClearAgent(); await RefreshAgentsAsync();
        });
    }
    private async Task DeleteAgentAsync()
    {
        if (currentAgentId == 0 || !Confirm("Delete this agent? Agents with orders or user accounts cannot be removed.")) return;
        await UseDb(async db =>
        {
            var entity = await db.Agents.FindAsync(currentAgentId);
            if (entity != null) { db.Agents.Remove(entity); await db.SaveChangesAsync(); }
            ClearAgent(); await RefreshAgentsAsync();
        });
    }

    private TabPage BuildOrdersTab()
    {
        var page = new TabPage("Order Management"); var bar = Bar();
        bar.Controls.AddRange(new Control[] { L("Update selected order to:"), statusPicker,
            B("Save status", async (_, _) => await SetOrderStatusAsync()),
            B("Reload", async (_, _) => await RefreshOrdersAsync()) });
        page.Controls.Add(ordersGrid); page.Controls.Add(bar);
        ordersGrid.SelectionChanged += (_, _) =>
        {
            var status = ordersGrid.CurrentRow?.Cells["OrderStatus"]?.Value?.ToString();
            if (!string.IsNullOrEmpty(status)) statusPicker.SelectedItem = status;
        };
        return page;
    }
    private async Task RefreshOrdersAsync() => await UseDb(async db =>
    {
        ordersGrid.DataSource = await db.Orders.AsNoTracking().OrderByDescending(x => x.OrderDate)
            .Select(o => new { o.OrderId, o.OrderDate, o.Agent.AgentName, o.OrderStatus,
                TotalVND = o.Details.Sum(d => (decimal?)(d.Quantity * d.UnitAmount)) ?? 0m })
            .ToListAsync();
    });
    private async Task SetOrderStatusAsync()
    {
        if (ordersGrid.CurrentRow is null) return;
        int id = Convert.ToInt32(ordersGrid.CurrentRow.Cells["OrderId"].Value);
        string status = statusPicker.SelectedItem!.ToString()!;
        await UseDb(async db =>
        {
            var order = await db.Orders.FindAsync(id) ?? throw new InvalidOperationException("Order not found.");
            order.OrderStatus = status; await db.SaveChangesAsync();
            await RefreshOrdersAsync();
        });
    }

    private TabPage BuildManifestTab()
    {
        var page = new TabPage("Shipping Manifest (PDF)"); var bar = Bar();
        bar.Controls.AddRange(new Control[] { L("Select an order from the grid below"),
            B("Export selected order as PDF", async (_, _) => await ExportManifestAsync()),
            B("Reload", async (_, _) => await RefreshManifestAsync()) });
        page.Controls.Add(manifestGrid); page.Controls.Add(bar); return page;
    }
    private async Task RefreshManifestAsync() => await UseDb(async db =>
    {
        manifestGrid.DataSource = await db.Orders.AsNoTracking().OrderByDescending(x => x.OrderDate)
            .Select(o => new { o.OrderId, o.OrderDate, o.Agent.AgentName, o.OrderStatus }).ToListAsync();
    });
    private async Task ExportManifestAsync()
    {
        if (manifestGrid.CurrentRow is null) return;
        int id = Convert.ToInt32(manifestGrid.CurrentRow.Cells["OrderId"].Value);
        await UseDb(async db =>
        {
            var order = await db.Orders.AsNoTracking().Include(o => o.Agent).Include(o => o.Details)
                .ThenInclude(d => d.Item).FirstOrDefaultAsync(o => o.OrderId == id)
                ?? throw new InvalidOperationException("Order not found.");
            using var dlg = new SaveFileDialog { Filter = "PDF document|*.pdf", FileName = $"ShippingManifest_{id}.pdf" };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            ManifestPdf.Generate(order, dlg.FileName);
            MessageBox.Show("Shipping Manifest exported to:\n" + dlg.FileName, "PDF export complete");
        });
    }

    private TabPage BuildDashboardTab()
    {
        var page = new TabPage("Sales Dashboard");
        var bar = Bar(); bar.Controls.AddRange(new Control[] { L("Low stock below"), lowStockThreshold,
            B("Refresh all reports", async (_, _) => await RefreshDashboardAsync()) });
        var split = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33)); split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        split.Controls.Add(Group("Best-selling items", bestSellersGrid), 0, 0);
        split.Controls.Add(Group("Low-stock warning", lowStockGrid), 1, 0);
        split.Controls.Add(Group("Revenue by agent (VND)", revenueGrid), 2, 0);
        page.Controls.Add(split); page.Controls.Add(bar); return page;
    }
    private static GroupBox Group(string title, Control control)
    {
        var g = new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(8, 16, 8, 8) };
        g.Controls.Add(control); return g;
    }
    private async Task RefreshDashboardAsync() => await UseDb(async db =>
    {
        bestSellersGrid.DataSource = await db.OrderDetails.AsNoTracking()
            .GroupBy(d => new { d.ItemId, d.Item.ItemName })
            .Select(g => new { Item = g.Key.ItemName, UnitsSold = g.Sum(d => d.Quantity) })
            .OrderByDescending(x => x.UnitsSold).ToListAsync();
        int cutoff = (int)lowStockThreshold.Value;
        lowStockGrid.DataSource = await db.Items.AsNoTracking().Where(i => i.StockQuantity < cutoff)
            .OrderBy(i => i.StockQuantity)
            .Select(i => new { Item = i.ItemName, Available = i.StockQuantity }).ToListAsync();
        revenueGrid.DataSource = await db.Agents.AsNoTracking().Select(a => new
        {
            Agent = a.AgentName,
            Revenue = a.Orders.SelectMany(o => o.Details).Sum(d => (decimal?)(d.Quantity * d.UnitAmount)) ?? 0m
        }).OrderByDescending(x => x.Revenue).ToListAsync();
    });
}
