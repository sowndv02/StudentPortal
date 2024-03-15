using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.Classes
{
    public class EditModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        private readonly int CLASS_MANAGER = (int)((UserRole[])Enum.GetValues(typeof(UserRole))).Where(role => role == UserRole.CLASS_MANAGER).ToArray().First();


        public EditModel(SEP_Management.Models.SEP_ManagementContext context)
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

            var classObject = await _context.Classes.FirstOrDefaultAsync(m => m.ClassId == id);
            if (classObject == null)
            {
                return NotFound();
            }
            var roleClassManagerId = _context.Roles.FirstOrDefault(x => x.DisplayOrder == CLASS_MANAGER).Id;
            ViewData["Status"] = new SelectList(ClassActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            ViewData["Managers"] = new SelectList(_context.Users.Where(x => x.RoleId.Equals(roleClassManagerId)), "Id", "FullName");
            ViewData["Subjects"] = new SelectList(_context.Subjects.ToList(), "SubjectId", "SubjectCode");
            ViewData["Semesters"] = new SelectList(_context.Roles.Where(x => x.DisplayOrder == null).ToList(), "Id", "Name");
            Class = classObject;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var classObject = _context.Classes.FirstOrDefault(x => x.ClassId == Class.ClassId);
            if (classObject == null)
            {
                return NotFound();
            }
            var checkClassCode = await _context.Classes.FirstOrDefaultAsync(x => x.ClassCode.Equals(Class.ClassCode) && x.ClassId != Class.ClassId);
            if (checkClassCode != null)
            {
                return NotFound();
            }
            classObject.SubjectId = Class.SubjectId;
            classObject.SemesterId = Class.SemesterId;
            classObject.Status = Class.Status;
            classObject.ClassCode = Class.ClassCode;
            classObject.ClassDetails = Class.ClassDetails;
            classObject.ManagerId = Class.ManagerId;
            //_context.Attach(Class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(Class.ClassId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ClassExists(int id)
        {
            return (_context.Classes?.Any(e => e.ClassId == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> OnGetChangeStatusAsync(string status, int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var model = await _context.Classes.FirstOrDefaultAsync(m => m.ClassId == id);
            if (model == null)
            {
                return NotFound();
            }
            Class = model;
            if (status.ToUpper().Equals("Active".ToUpper()))
            {
                Class.Status = 0;
                model.Status = 0;
            }
            else if (status.ToUpper().Equals("PENDING".ToUpper()))
            {
                Class.Status = 1;
                model.Status = 1;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(Class.ClassId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("./Index");
        }

    }
}
