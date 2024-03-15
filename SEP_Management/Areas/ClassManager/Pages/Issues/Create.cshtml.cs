using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SEP_Management.Enums;
using SEP_Management.Models;
using SEP_Management.Utils;

namespace SEP_Management.Areas.ClassManager.Pages.Issues
{
    public class CreateModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public CreateModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? projectId)
        {
            string tmp = null;
            var labels = _context.IssueSettings.Where(x => x.ProjectId == projectId).ToList();
            foreach (var item in labels)
            {
                tmp = SyncGitLab.GetNamespaceAndSpecificLabel(item.Title, item.Status);
                if (tmp != null) item.Title = tmp;
            }
            Project = _context.Projects.FirstOrDefault(p => p.ProjectId == projectId);
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            ViewData["Process"] = new SelectList(ProcessEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayProcessName");
            ViewData["Milestones"] = new SelectList(_context.Milestones.Where(x => x.ProjectId == projectId).ToList(), "MilestoneId", "MilestoneName");
            ViewData["Labels"] = new SelectList(labels, "SettingId", "Title");
            //ViewData["Assignees"] = new SelectList(_context.ClassStudents.Where(x => x.ProjectId == projectId).ToList(), "Id", "FullName");
            return Page();
        }

        [BindProperty]
        public Issue Issue { get; set; } = default!;
        public Project Project { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Issues == null || Issue == null)
            {
                return Page();
            }

            _context.Issues.Add(Issue);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
