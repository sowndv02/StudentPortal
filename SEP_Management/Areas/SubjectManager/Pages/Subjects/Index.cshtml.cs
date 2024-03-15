using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.SubjectManager.Pages.Subjects
{
    public class IndexModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public IndexModel(SEP_Management.Models.SEP_ManagementContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IList<Subject> Subject { get; set; } = default!;

        public async Task OnGetAsync(string? subjectCode)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (_context.Subjects != null && user != null)
            {
                Subject = await _context.Subjects
                .Include(s => s.Manager).Where(x => x.ManagerId.Equals(user.Id)).ToListAsync();
            }
        }
    }
}
