using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.ClassManager.Pages.Classes
{
    public class IndexModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(SEP_Management.Models.SEP_ManagementContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<Class> Classes { get; set; } = default!;
        [BindProperty]
        public Class Class { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (_context.Classes != null && user != null)
            {
                Classes = await _context.Classes.Include(x => x.Semester).Include(x => x.Subject).Where(x => x.ManagerId.Equals(user.Id)).ToListAsync();
            }
        }

    }
}
