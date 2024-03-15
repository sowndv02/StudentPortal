using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Pages.Projects
{
    public class DetailsModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        private readonly UserManager<User> _userManager;
        public DetailsModel(SEP_Management.Models.SEP_ManagementContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? projectId, int? classId)
        {
            if (projectId == null || classId == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FirstOrDefaultAsync(m => m.ProjectId == projectId && m.ClassId == classId);
            if (project == null)
            {
                return NotFound();
            }
            Project = project;
            ViewData["Classes"] = new SelectList(_context.Classes, "ClassId", "ClassCode");

            var allUsers = await _userManager.Users.ToListAsync();
            ViewData["Mentors"] = new SelectList(allUsers.Where(u =>
            _userManager.GetClaimsAsync(u).Result.Any(c =>
            c.Type == "Role" && c.Value == "PROJECT_MENTOR")), "Id", "FullName");

            return Page();
        }
    }
}
