using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.Classes
{
    public class DetailsModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public DetailsModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Class Class { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var classObject = await _context.Classes.Include(x => x.Subject).FirstOrDefaultAsync(m => m.ClassId == id);
            if (classObject == null)
            {
                return NotFound();
            }
            else
            {
                Class = classObject;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Class == null) return NotFound();
            var classObject = await _context.Classes.FirstOrDefaultAsync(x => x.ClassId == Class.ClassId);
            if (classObject == null)
            {
                return NotFound();
            }
            classObject.AccessToken = Class.AccessToken;
            classObject.GitlabId = Class.GitlabId;
            classObject.ClassDetails = Class.ClassDetails;
            await _context.SaveChangesAsync();
            return Page();
        }
    }
}
