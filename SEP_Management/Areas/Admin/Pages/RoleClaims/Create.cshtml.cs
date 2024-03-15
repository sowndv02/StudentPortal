using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.RoleClaims
{
    public class CreateModel : PageModel
    {

        private readonly SEP_ManagementContext _context;
        private readonly RoleManager<Role> _roleManager;
        public Role Role { set; get; }

        public CreateModel(SEP_ManagementContext context, RoleManager<Role> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> OnGet(string? roleId)
        {
            if (roleId == null)
            {
                return NotFound();
            }
            Role = await _roleManager.FindByIdAsync(roleId);

            if (Role == null)
                return NotFound();
            return Page();
        }



        [BindProperty(SupportsGet = true)]
        public string roleId { set; get; }

        [BindProperty]
        public IdentityRoleClaim<string> RoleClaim { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Role = await _roleManager.FindByIdAsync(roleId);
            if (Role == null)
                return NotFound("Not Found");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            RoleClaim.RoleId = roleId;

            _context.RoleClaims.Add(RoleClaim);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { roleid = roleId });
        }
    }
}
