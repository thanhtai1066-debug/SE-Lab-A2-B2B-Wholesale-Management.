using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wholesale.Data;

namespace Wholesale.Web.Controllers;

[Authorize(Roles = "Agent")]
public class CatalogController(WholesaleDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Items.AsNoTracking()
        .OrderBy(x => x.ItemName).ToListAsync());
}
