using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;
using SEP_Management.Utils;

namespace SEP_Management.Areas.ClassManager.Pages.Issues
{
    public class EditModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public EditModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Issue Issue { get; set; } = default!;
        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Issues == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues.Include(x => x.Project).FirstOrDefaultAsync(m => m.IssueId == id);
            if (issue == null)
            {
                return NotFound();
            }
            Issue = issue;
            Project = Issue.Project;
            ViewData["MilestoneId"] = new SelectList(_context.Milestones, "MilestoneId", "MilestoneId");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId");
            ViewData["TypeId"] = new SelectList(_context.IssueSettings, "SettingId", "SettingId");
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(Issue.IssueId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetChangeStatusAsync(string status, int? id)
        {
            if (id == null || _context.IssueSettings == null)
            {
                return NotFound();
            }

            var model = await _context.Issues.FirstOrDefaultAsync(m => m.IssueId == id);
            if (model == null)
            {
                return NotFound();
            }
            Issue = model;
            if (status.ToUpper().Equals("InActive".ToUpper()))
            {
                Issue.StatusId = 1;
                model.StatusId = 1;
            }
            else if (status.ToUpper().Equals("Active".ToUpper()))
            {
                Issue.StatusId = 0;
                model.StatusId = 0;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(Issue.IssueId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Redirect($"/ClassManager/Issues?projectId={Issue.ProjectId}");
        }

        public async Task<IActionResult> OnPostSyncGitLabAsync()
        {
            Project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectId == Project.ProjectId);
            if (Project == null)
            {
                return NotFound();
            }

            await SyncGitLab.SyncLabelsWithProject(Project);
            return Redirect($"/ClassManager/IssueSettings?projectId={Project.ProjectId}");

        }

        private bool IssueExists(int id)
        {
            return (_context.Issues?.Any(e => e.IssueId == id)).GetValueOrDefault();
        }
    }
}
