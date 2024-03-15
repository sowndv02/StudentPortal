using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;

namespace SEP_Management.Areas.ClassManager.Pages.Milestones
{
    public class CreateModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        public Project Project { get; set; }
        public CreateModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(int? projectId)
        {
            if (projectId == null || _context.Projects == null)
            {
                return NotFound();
            }
            Project = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectId == projectId);
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            return Page();
        }

        [BindProperty]
        public Milestone Milestone { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Milestone.CreatedAt = DateTime.Now;
            Milestone.UpdatedAt = DateTime.Now;
            if (!ModelState.IsValid || _context.Milestones == null || Milestone == null)
            {
                return Page();
            }
            var checkTitle = _context.Milestones.FirstOrDefault(x => x.MilestoneName.Equals(Milestone.MilestoneName));
            if (checkTitle != null)
            {
                return NotFound();
            }
            if (Milestone.StartDate > Milestone.DueDate)
            {
                var tmp = Milestone.StartDate;
                Milestone.StartDate = Milestone.DueDate;
                Milestone.DueDate = tmp;
            }
            _context.Milestones.Add(Milestone);
            await _context.SaveChangesAsync();

            return Redirect($"/ClassManager/Milestones?projectId={Milestone.ProjectId}");
        }
    }
}
