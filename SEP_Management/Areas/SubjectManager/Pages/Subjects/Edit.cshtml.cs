using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;

namespace SEP_Management.Areas.SubjectManager.Pages.Subjects
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
        public Subject Subject { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FirstOrDefaultAsync(m => m.SubjectId == id);
            if (subject == null)
            {
                return NotFound();
            }
            Subject = subject;
            var roleManagerId = _context.Roles.FirstOrDefault(r => r.DisplayOrder == CLASS_MANAGER).Id;
            ViewData["Managers"] = new SelectList(_context.Users.Where(x => x.RoleId.Equals(roleManagerId)), "Id", "FullName");
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().Where(x => x.ActiveKey != -1).ToList(), "ActiveKey", "DisplayActiveName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            //_context.Attach(Subject).State = EntityState.Modified;


            var checkCode = _context.Subjects.FirstOrDefault(x => x.SubjectCode.Equals(Subject.SubjectCode) && x.SubjectId != Subject.SubjectId);
            if (checkCode != null)
            {
                return NotFound();
            }
            Subject subject = await _context.Subjects.FirstOrDefaultAsync(x => x.SubjectId == Subject.SubjectId);
            subject.SubjectName = Subject.SubjectName;
            subject.SubjectCode = Subject.SubjectCode;
            subject.Description = Subject.Description;
            subject.ManagerId = Subject.ManagerId;
            subject.IsActive = Subject.IsActive;
            subject.PassGrade = Subject.PassGrade;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(Subject.SubjectId))
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

        public async Task<IActionResult> OnGetChangeStatusAsync(string status, int? id)
        {
            if (id == null || _context.IssueSettings == null)
            {
                return NotFound();
            }

            var model = await _context.Subjects.FirstOrDefaultAsync(m => m.SubjectId == id);
            if (model == null)
            {
                return NotFound();
            }
            Subject = model;
            if (status.ToUpper().Equals("InActive".ToUpper()))
            {
                Subject.IsActive = 1;
                model.IsActive = 1;
            }
            else if (status.ToUpper().Equals("Active".ToUpper()))
            {
                Subject.IsActive = 0;
                model.IsActive = 0;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(Subject.SubjectId))
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

        private bool SubjectExists(int id)
        {
            return (_context.Subjects?.Any(e => e.SubjectId == id)).GetValueOrDefault();
        }
    }
}
