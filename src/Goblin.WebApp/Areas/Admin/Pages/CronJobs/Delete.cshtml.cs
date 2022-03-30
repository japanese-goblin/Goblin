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
public class Delete : PageModel
{
    public CronJob CronJob { get; set; }
    private readonly BotDbContext _context;

    public Delete(BotDbContext context)
    {
        _context = context;
    }

    public async Task OnGet(int id)
    {
        var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.Id == id);
        if(job is null)
        {
            return;
        }

        CronJob = job;
    }

    public async Task<IActionResult> OnPost(int id)
    {
        var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.Id == id);
        if(job is null)
        {
            return Page();
        }

        _context.Remove(job);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}