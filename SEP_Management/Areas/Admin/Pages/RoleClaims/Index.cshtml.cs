using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.RoleClaims
{
    public class IndexModel : PageModel
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly SEP_ManagementContext _dbContext;

        public IndexModel(RoleManager<Role> roleManager, SEP_ManagementContext appDbContext)
        {
            _dbContext = appDbContext;
            _roleManager = roleManager;
        }
        public List<Role> Roles { set; get; }

        public string roleId { set; get; }
        [BindProperty]
        public Role Role { set; get; }

        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        public IdentityRoleClaim<string> Claim { set; get; }
        public IList<RoleClaim> claims { get; set; }

        public async Task<IActionResult> OnGet(string roleId)
        {
            Role = await _roleManager.FindByIdAsync(roleId);

            if (Role == null)
                return NotFound("Not Found");

            claims = await _dbContext.RoleClaims
            .Where(c => c.RoleId == roleId)
            .Select(c => new RoleClaim
            {
                Id = c.Id,
                ClaimType = c.ClaimType,
                ClaimValue = c.ClaimValue
            })
            .ToListAsync();


            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (Claim == null)
            {
                return NotFound();
            }

            Claim = await _dbContext.RoleClaims.FindAsync(Claim.Id);

            if (Claim != null)
            {
                _dbContext.RoleClaims.Remove(Claim);
                await _dbContext.SaveChangesAsync();
            }

            return Redirect($"/Admin/Claims?roleId={Role.Id}");
        }
    }
}
