using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.RoleClaims
{
    public class EditModel : PageModel
    {
        private readonly SEP_ManagementContext _context;
        private readonly RoleManager<Role> _roleManager;
        public Role Role { set; get; }

        [BindProperty(SupportsGet = true)]
        public string roleId { set; get; }
        public EditModel(SEP_ManagementContext context, RoleManager<Role> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }


        [BindProperty]
        public IdentityRoleClaim<string> RoleClaim { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RoleClaim = await _context.RoleClaims.FirstOrDefaultAsync(m => m.Id == id);
            Role = await _roleManager.FindByIdAsync(RoleClaim.RoleId);
            if (RoleClaim == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Role = await _roleManager.FindByIdAsync(RoleClaim.RoleId);
            if (Role == null)
                return NotFound("Not Found");

            var claim = await _context.RoleClaims.FirstOrDefaultAsync(x => x.Id == RoleClaim.Id);
            if (claim == null)
                return NotFound();
            claim.ClaimValue = RoleClaim.ClaimValue;
            claim.ClaimType = RoleClaim.ClaimType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleClaimExists(RoleClaim.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { roleid = RoleClaim.RoleId });
        }

        private bool RoleClaimExists(int id)
        {
            return _context.RoleClaims.Any(e => e.Id == id);
        }
    }
}
