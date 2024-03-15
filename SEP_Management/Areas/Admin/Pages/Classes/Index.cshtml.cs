﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;

namespace SEP_Management.Areas.Admin.Pages.Classes
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
            if (_context.Classes != null)
            {
                Classes = await _context.Classes.Include(x => x.Semester).Include(x => x.Manager).Include(x => x.Subject).ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (Class == null || _context.Classes == null)
            {
                return NotFound();
            }
            var classObject = await _context.Classes.FindAsync(Class.ClassId);

            if (classObject != null)
            {
                Class = classObject;
                _context.Classes.Remove(classObject);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
