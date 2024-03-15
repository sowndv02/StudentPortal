using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;

namespace SEP_Management.Areas.ClassManager.Pages.IssueSettings
{
    public class CreateModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public CreateModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? projectId)
        {
            if (projectId == null)
            {
                return NotFound();
            }
            Project = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectId == projectId);
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            return Page();
        }

        [BindProperty]
        public IssueSetting IssueSetting { get; set; } = default!;
        public Project Project { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            IssueSetting.UpdatedAt = DateTime.Now;
            IssueSetting.CreatedAt = DateTime.Now;
            if (!ModelState.IsValid || _context.IssueSettings == null || IssueSetting == null)
            {
                return Page();
            }
            if (string.IsNullOrEmpty(IssueSetting.Status) && string.IsNullOrEmpty(IssueSetting.Title))
            {
                return NotFound();
            }
            var checkTile = await _context.IssueSettings.FirstOrDefaultAsync(x => x.Status.Equals(IssueSetting.Status) && x.Title.Equals(IssueSetting.Title));
            if (checkTile != null)
            {
                return NotFound();
            }
            _context.IssueSettings.Add(IssueSetting);
            await _context.SaveChangesAsync();

            return Redirect($"/ClassManager/IssueSettings?projectId={IssueSetting.ProjectId}");
        }
    }
}
