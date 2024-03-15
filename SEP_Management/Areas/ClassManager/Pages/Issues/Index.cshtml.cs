using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.ClassManager.Pages.Issues
{
    public class IndexModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public IndexModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        public IList<Issue> Issue { get; set; } = default!;
        public Project Project { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync(int? projectId)
        {
            if (projectId == null)
            {
                return NotFound();

            }
            if (_context.Issues != null)
            {
                Issue = await _context.Issues
                .Include(i => i.Milestone)
                .Include(i => i.Project)
                .Where(x => x.ProjectId == projectId).ToListAsync();
                Project = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectId == projectId);
            }
            return Page();
        }
    }
}
