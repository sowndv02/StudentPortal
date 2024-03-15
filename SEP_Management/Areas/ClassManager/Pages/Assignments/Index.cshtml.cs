using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.ClassManager.Pages.Assignments
{
    public class IndexModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public IndexModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        public IList<Assignment> Assignment { get; set; } = default!;
        public Subject Subject { get; set; }

        public async Task OnGetAsync(int? subjectId)
        {
            if (_context.Assignments != null)
            {
                Assignment = await _context.Assignments
                .Include(a => a.Subject).Where(x => x.SubjectId == subjectId).ToListAsync();
                Subject = await _context.Subjects.FindAsync(subjectId);
            }
        }
    }
}
