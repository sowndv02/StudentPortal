using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Enums;
using SEP_Management.Models;

namespace SEP_Management.Areas.SubjectManager.Pages.SubjectSettings
{
    public class IndexModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;

        public IndexModel(SEP_Management.Models.SEP_ManagementContext context)
        {
            _context = context;
        }

        public IList<SubjectSetting> SubjectSetting { get; set; } = default!;

        public Subject Subject { get; set; }
        public int StatusFilter { get; set; } = int.MinValue;
        public int IssueTypeFilter { get; set; } = int.MinValue;

        public async Task OnGetAsync(int subjectId, int? statusFilter, int? issueTypeFilter)
        {
            if (_context.SubjectSettings != null)
            {
                SubjectSetting = await _context.SubjectSettings
                .Include(s => s.Subject).Where(x => x.SubjectId == subjectId).ToListAsync();
                if (statusFilter != null && statusFilter != int.MinValue && statusFilter != -1)
                {
                    SubjectSetting = SubjectSetting.Where(s => s.SettingValue == statusFilter).ToList();
                }
                if (issueTypeFilter != null && issueTypeFilter != int.MinValue && issueTypeFilter != -1)
                {
                    SubjectSetting = SubjectSetting.Where(s => s.SettingType == issueTypeFilter).ToList();
                }

                Subject = await _context.Subjects.FindAsync(subjectId);
            }
            ViewData["Status"] = new SelectList(ActiveEnum.GetListActive().ToList(), "ActiveKey", "DisplayActiveName");
        }
    }
}
