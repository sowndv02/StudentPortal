using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.Classes
{
    public class CreateModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        private readonly int CLASS_MANAGER = (int)((UserRole[])Enum.GetValues(typeof(UserRole))).Where(role => role == UserRole.CLASS_MANAGER).ToArray().First();

        public CreateModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var roleClassManagerId = _context.Roles.FirstOrDefault(x => x.DisplayOrder == CLASS_MANAGER).Id;
            ViewData["Status"] = new SelectList(ClassActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            ViewData["Managers"] = new SelectList(_context.Users.Where(x => x.RoleId.Equals(roleClassManagerId)), "Id", "FullName");
            ViewData["Subjects"] = new SelectList(_context.Subjects.ToList(), "SubjectId", "SubjectCode");
            ViewData["Semesters"] = new SelectList(_context.Roles.Where(x => x.DisplayOrder == null).ToList(), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Class Class { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Classes == null || Class == null)
            {
                return Page();
            }
            var checkClassCode = await _context.Classes.FirstOrDefaultAsync(x => x.ClassCode.Equals(Class.ClassCode));
            if (checkClassCode != null)
            {
                return NotFound();
            }
            _context.Classes.Add(Class);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
