using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Areas.Admin.Pages.CronJobs;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class Index : PageModel
{
    public CronJob[] CronJobs { get; set; }
    private readonly BotDbContext _context;

    public Index(BotDbContext context)
    {
        _context = context;
    }

    public async Task OnGet()
    {
        CronJobs = await _context.CronJobs.ToArrayAsync();
    }
}