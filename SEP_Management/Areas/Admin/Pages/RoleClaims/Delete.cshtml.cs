using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.RoleClaims
{
    public class DeleteModel : PageModel
    {

        private readonly SEP_ManagementContext _context;
        private readonly RoleManager<Role> _roleManager;

        public IdentityRole role { set; get; }

        [BindProperty(SupportsGet = true)]
        public string roleid { set; get; }

        public DeleteModel(SEP_ManagementContext context, RoleManager<Role> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        async Task<Role> GetRole()
        {

            if (string.IsNullOrEmpty(roleid)) return null;
            return await _roleManager.FindByIdAsync(roleid);
        }

        [BindProperty]
        public IdentityRoleClaim<string> RoleClaim { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            role = await GetRole();
            if (role == null)
                return NotFound("Không thấy Role");

            if (id == null)
            {
                return NotFound();
            }

            RoleClaim = await _context.RoleClaims.FirstOrDefaultAsync(m => m.Id == id);

            if (RoleClaim == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {

            role = await GetRole();
            if (role == null)
                return NotFound("Không thấy Role");

            if (id == null)
            {
                return NotFound();
            }

            RoleClaim = await _context.RoleClaims.FindAsync(id);

            if (RoleClaim != null)
            {
                _context.RoleClaims.Remove(RoleClaim);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { roleid = roleid });
        }

    }
}
