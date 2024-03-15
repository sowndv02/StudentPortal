using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;
using SEP_Management.Utils;

namespace SEP_Management.Areas.ClassManager.Pages.IssueSettings
{
    public class EditModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public EditModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IssueSetting IssueSetting { get; set; } = default!;
        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.IssueSettings == null)
            {
                return NotFound();
            }

            var issuesetting = await _context.IssueSettings.Include(x => x.Project).FirstOrDefaultAsync(m => m.SettingId == id);
            if (issuesetting == null)
            {
                return NotFound();
            }
            IssueSetting = issuesetting;
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            Project = issuesetting.Project;
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

            //_context.Attach(IssueSetting).State = EntityState.Modified;
            if (string.IsNullOrEmpty(IssueSetting.Status) && string.IsNullOrEmpty(IssueSetting.Title))
            {
                return NotFound();
            }
            var checkTile = await _context.IssueSettings.FirstOrDefaultAsync(x => x.Status.Equals(IssueSetting.Status) && x.Title.Equals(IssueSetting.Title) && x.SettingId != IssueSetting.SettingId);
            if (checkTile != null)
            {
                return NotFound();
            }
            var issueSetting = await _context.IssueSettings.FirstOrDefaultAsync(x => x.SettingId == IssueSetting.SettingId);
            issueSetting.Status = IssueSetting.Status;
            issueSetting.Title = IssueSetting.Title;
            issueSetting.Description = IssueSetting.Description;
            issueSetting.IsActive = IssueSetting.IsActive;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueSettingExists(IssueSetting.SettingId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect($"/ClassManager/IssueSettings?projectId={IssueSetting.ProjectId}");
        }
        public async Task<IActionResult> OnGetChangeStatusAsync(string status, int? id)
        {
            if (id == null || _context.IssueSettings == null)
            {
                return NotFound();
            }

            var model = await _context.IssueSettings.FirstOrDefaultAsync(m => m.SettingId == id);
            if (model == null)
            {
                return NotFound();
            }
            IssueSetting = model;
            if (status.ToUpper().Equals("InActive".ToUpper()))
            {
                IssueSetting.IsActive = 1;
                model.IsActive = 1;
            }
            else if (status.ToUpper().Equals("Active".ToUpper()))
            {
                IssueSetting.IsActive = 0;
                model.IsActive = 0;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueSettingExists(IssueSetting.SettingId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Redirect($"/ClassManager/IssueSettings?projectId={IssueSetting.ProjectId}");
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

        private bool IssueSettingExists(int id)
        {
            return (_context.IssueSettings?.Any(e => e.SettingId == id)).GetValueOrDefault();
        }
    }
}
