using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Wholesale.Data;

namespace Wholesale.Desktop;
public static class ManifestPdf
{
    public static void Generate(SalesOrder order, string output)
    {
        QuestPDF.Settings.License = LicenseType.Community; // Educational demonstration.
        Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4); page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(11));
            page.Header().Column(head =>
            {
                head.Item().Text("SHIPPING MANIFEST").Bold().FontSize(22).FontColor(Colors.Blue.Darken3);
                head.Item().Text("B2B Wholesale Order Management System").FontSize(10);
            });
            page.Content().PaddingVertical(20).Column(c =>
            {
                c.Spacing(12);
                c.Item().Text($"Manifest for order #{order.OrderId} • Status: {order.OrderStatus}").Bold();
                c.Item().Text($"Order created (UTC): {order.OrderDate:yyyy-MM-dd HH:mm}");
                c.Item().Text($"Ship to: {order.Agent.AgentName}").Bold();
                c.Item().Text(order.Agent.Address);
                c.Item().Text($"Telephone: {order.Agent.Phone}");
                c.Item().Table(t =>
                {
                    t.ColumnsDefinition(cols => { cols.RelativeColumn(5); cols.RelativeColumn(2); cols.RelativeColumn(1); });
                    t.Header(h =>
                    {
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(7).Text("Item").Bold();
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(7).Text("Pack / size").Bold();
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(7).AlignRight().Text("Qty").Bold();
                    });
                    foreach (var d in order.Details)
                    {
                        t.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(7).Text(d.Item.ItemName);
                        t.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(7).Text(d.Item.Size);
                        t.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(7).AlignRight().Text(d.Quantity.ToString());
                    }
                });
                c.Item().AlignRight().Text($"Total units: {order.Details.Sum(x => x.Quantity)}").Bold();
                c.Item().PaddingTop(28).Text("Packed by: ____________________    Checked by: ____________________");
            });
            page.Footer().AlignCenter().Text("SE Lab Assignment 2 | Local demo shipping document").FontSize(9);
        })).GeneratePdf(output);
    }
}
