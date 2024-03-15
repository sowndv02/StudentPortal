using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.SubjectManager.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        private readonly UserManager<User> _userManager;
        public CreateModel(SEP_Management.Models.SEP_ManagementContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ClassLists"] = new SelectList(_context.Classes, "ClassId", "ClassCode");
            var allUsers = await _userManager.Users.ToListAsync();
            ViewData["MentorList"] = new SelectList(allUsers.Where(u =>
            _userManager.GetClaimsAsync(u).Result.Any(c =>
            c.Type == "Role" && c.Value == "PROJECT_MENTOR")), "Id", "FullName");
            return Page();
        }

        [BindProperty]
        public Project Project { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Project.Status = 0;
            if (!ModelState.IsValid || _context.Projects == null || Project == null)
            {
                return Page();
            }
            var checkProjectCode = _context.Projects.FirstOrDefault(x => x.ProjectCode.ToUpper().Equals(Project.ProjectCode.ToUpper()));
            if (checkProjectCode != null)
            {
                return Page();
            }
            Project.ProjectCode = Project.ProjectCode.ToUpper();
            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
