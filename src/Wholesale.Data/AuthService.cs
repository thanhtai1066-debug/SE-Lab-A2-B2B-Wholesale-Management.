using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wholesale.Core;

namespace Wholesale.Data;

public enum LoginOutcome { Success, InvalidCredentials, Locked, ForbiddenRole }
public sealed record LoginResult(LoginOutcome Outcome, UserAccount? User = null);

public class AuthService(WholesaleDbContext db)
{
    private readonly PasswordHasher<UserAccount> hasher = new();

    public async Task<LoginResult> AuthenticateAsync(string? userName, string? password, string requiredRole, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
            return new LoginResult(LoginOutcome.InvalidCredentials);
        var user = await db.Users.SingleOrDefaultAsync(x => x.UserName == userName.Trim(), ct);
        if (user == null) return new LoginResult(LoginOutcome.InvalidCredentials);
        if (user.IsLocked) return new LoginResult(LoginOutcome.Locked);

        PasswordVerificationResult verified;
        try { verified = hasher.VerifyHashedPassword(user, user.PasswordHash, password); }
        catch (FormatException) { verified = PasswordVerificationResult.Failed; }
        if (verified == PasswordVerificationResult.Failed)
        {
            user.FailedLoginCount++;
            if (OrderRules.MustLock(user.FailedLoginCount)) user.IsLocked = true;
            await db.SaveChangesAsync(ct);
            return new LoginResult(user.IsLocked ? LoginOutcome.Locked : LoginOutcome.InvalidCredentials);
        }
        user.FailedLoginCount = 0;
        if (verified == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = hasher.HashPassword(user, password);
        await db.SaveChangesAsync(ct);
        if (user.Role != requiredRole || (requiredRole == "Agent" && !user.AgentId.HasValue))
            return new LoginResult(LoginOutcome.ForbiddenRole);
        return new LoginResult(LoginOutcome.Success, user);
    }
}
