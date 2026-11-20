using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wholesale.Data;
using Wholesale.Web.Models;

namespace Wholesale.Web.Controllers;

public class AccountController(AuthService auth) : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.IsInRole("Agent")) return RedirectToAction("Index", "Catalog");
        return View(new LoginVm { ReturnUrl = returnUrl });
    }

    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await auth.AuthenticateAsync(model.UserName, model.Password, "Agent");
        if (result.Outcome != LoginOutcome.Success || result.User is null)
        {
            ModelState.AddModelError("", result.Outcome switch
            {
                LoginOutcome.Locked => "Account locked after too many failed attempts.",
                LoginOutcome.ForbiddenRole => "Only Agent accounts may access this portal.",
                _ => "Incorrect username or password."
            });
            return View(model);
        }

        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, result.User.UserId.ToString()),
            new(ClaimTypes.Name, result.User.UserName),
            new(ClaimTypes.Role, "Agent"),
            new("AgentId", result.User.AgentId!.Value.ToString())
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return LocalRedirect(model.ReturnUrl);
        return RedirectToAction("Index", "Catalog");
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [AllowAnonymous]
    public IActionResult Denied() => View();
}
