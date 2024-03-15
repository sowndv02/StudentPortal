using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;
using SEP_Management.Utils;

namespace SEP_Management.Areas.ClassManager.Pages.Milestones
{
    public class EditModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public EditModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Milestone Milestone { get; set; } = default!;

        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones.FirstOrDefaultAsync(m => m.MilestoneId == id);
            if (milestone == null)
            {
                return NotFound();
            }
            Milestone = milestone;
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            return Page();
        }

        public async Task<IActionResult> OnGetChangeStatusAsync(string status, int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones.FirstOrDefaultAsync(m => m.MilestoneId == id);
            if (milestone == null)
            {
                return NotFound();
            }
            Milestone = milestone;
            if (status.ToUpper().Equals("InActive".ToUpper()))
            {
                Milestone.IsActive = 1;
                milestone.IsActive = 1;
            }
            else if (status.ToUpper().Equals("Active".ToUpper()))
            {
                Milestone.IsActive = 0;
                milestone.IsActive = 0;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MilestoneExists(Milestone.MilestoneId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Redirect($"/ClassManager/Milestones?projectId={Milestone.ProjectId}");
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Milestone.CreatedAt = DateTime.Now;
            Milestone.UpdatedAt = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var checkTitle = _context.Milestones.FirstOrDefault(x => x.MilestoneName.ToUpper().Equals(Milestone.MilestoneName.ToUpper()));
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
            _context.Attach(Milestone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MilestoneExists(Milestone.MilestoneId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect($"/ClassManager/Milestones?projectId={Milestone.ProjectId}");
        }

        public async Task<IActionResult> OnPostSyncGitLabAsync()
        {
            Project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectId == Project.ProjectId);
            if (Project == null)
            {
                return NotFound();
            }

            await SyncGitLab.SyncMilestonesWithProject(Project);
            return Redirect($"/ClassManager/Milestones?projectId={Project.ProjectId}");

        }
        private bool MilestoneExists(int id)
        {
            return (_context.Milestones?.Any(e => e.MilestoneId == id)).GetValueOrDefault();
        }
    }
}
