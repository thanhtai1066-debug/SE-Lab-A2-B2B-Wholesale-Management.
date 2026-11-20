using Microsoft.EntityFrameworkCore;
using Wholesale.Data;

namespace Wholesale.Desktop;
public sealed class LoginForm : Form
{
    private readonly IDbContextFactory<WholesaleDbContext> factory;
    private readonly TextBox userBox = new() { Width = 280, Text = "admin" };
    private readonly TextBox passwordBox = new() { Width = 280, UseSystemPasswordChar = true };
    private readonly Label message = new() { ForeColor = Color.DarkRed, AutoSize = true };
    private readonly Button signIn = new() { Text = "Sign in as Admin", Width = 280, Height = 38 };
    public UserAccount? LoggedAdmin { get; private set; }

    public LoginForm(IDbContextFactory<WholesaleDbContext> factory)
    {
        this.factory = factory;
        Text = "Wholesale Back-Office | Admin Login"; Size = new Size(440, 365);
        StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; BackColor = Color.FromArgb(244, 247, 251);
        var column = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(48, 30, 30, 15), AutoScroll = true };
        column.Controls.Add(new Label { Text = "WAREHOUSE BACK-OFFICE", Font = new Font("Segoe UI", 17, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(25, 57, 95) });
        column.Controls.Add(new Label { Text = "Secure administrator access", AutoSize = true, Margin = new Padding(0, 5, 0, 18) });
        column.Controls.Add(new Label { Text = "Username", AutoSize = true }); column.Controls.Add(userBox);
        column.Controls.Add(new Label { Text = "Password", AutoSize = true, Margin = new Padding(0, 10, 0, 3) });
        column.Controls.Add(passwordBox); column.Controls.Add(signIn);
        column.Controls.Add(message);
        column.Controls.Add(new Label { Text = "Local demo: admin / Admin@12345!", AutoSize = true, ForeColor = Color.DimGray });
        Controls.Add(column);
        AcceptButton = signIn;
        signIn.Click += async (_, _) => await SignInAsync();
    }

    private async Task SignInAsync()
    {
        signIn.Enabled = false;
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            var result = await new AuthService(db).AuthenticateAsync(userBox.Text, passwordBox.Text, "Admin");
            if (result.Outcome == LoginOutcome.Success)
            {
                LoggedAdmin = result.User;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            message.Text = result.Outcome switch
            {
                LoginOutcome.Locked => "Account is locked. Contact an administrator.",
                LoginOutcome.ForbiddenRole => "Only Admin users can access this application.",
                _ => "Invalid username or password."
            };
        }
        catch (Exception ex) { message.Text = "Sign in failed: " + ex.Message; }
        finally { signIn.Enabled = true; }
    }
}
