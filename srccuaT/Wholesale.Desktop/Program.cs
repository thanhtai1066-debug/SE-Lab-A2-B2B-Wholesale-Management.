using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Wholesale.Data;

namespace Wholesale.Desktop;
internal static class Program
{
    [STAThread]
    private static async Task Main()
    {
        ApplicationConfiguration.Initialize();
        var cs = ConfigurationManager.ConnectionStrings["WholesaleDatabase"]?.ConnectionString
            ?? throw new InvalidOperationException("Set WholesaleDatabase in App.config.");
        var options = new DbContextOptionsBuilder<WholesaleDbContext>().UseSqlServer(cs).Options;
        var factory = new DesktopContextFactory(options);
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
            await DemoSeeder.SeedAsync(db);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database initialization failed. Check SQL Server/LocalDB and App.config.\n\n" + ex.Message,
                "Database unavailable", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        using var login = new LoginForm(factory);
        if (login.ShowDialog() == DialogResult.OK && login.LoggedAdmin != null)
            Application.Run(new MainForm(factory, login.LoggedAdmin));
    }
}
internal sealed class DesktopContextFactory(DbContextOptions<WholesaleDbContext> options) : IDbContextFactory<WholesaleDbContext>
{
    public WholesaleDbContext CreateDbContext() => new(options);
}
