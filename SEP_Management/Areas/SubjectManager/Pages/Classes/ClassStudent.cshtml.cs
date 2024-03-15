using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SEP_Management.Models;
using SEP_Management.Utils;
using SEP_Management.Validators;
using System.ComponentModel.DataAnnotations;

namespace SEP_Management.Areas.SubjectManager.Pages.Classes
{
    public class ClassStudentModel : PageModel
    {
        private readonly SEP_Management.Models.SEP_ManagementContext _context;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        [DataType(DataType.Upload)]
        [CheckFileExtensions(Extensions = "xlxs")]
        public IFormFile FileUpload { get; set; }
        private readonly ExcelHandler _excelHandler;
        public ClassStudentModel(SEP_Management.Models.SEP_ManagementContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _excelHandler = new ExcelHandler();
        }
        public IList<ClassStudent> ClassStudents { get; set; }
        [BindProperty]
        public Class Class { get; set; }
        public async Task<IActionResult> OnGetAsync(int? classId)
        {
            if (classId == null) return NotFound();
            var classStudent = await _context.ClassStudents.Include(x => x.Class).Include(x => x.Project).Include(x => x.Student).Where(x => x.ClassId == classId).ToListAsync();
            if (classStudent == null) return NotFound();
            ClassStudents = classStudent;
            Class = await _context.Classes.FindAsync(classId);
            return Page();
        }


        public async Task<IActionResult> OnGetExportAsync(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }
            var list = await _context.ClassStudents.Where(x => x.ClassId == id).Include(x => x.Class).Include(x => x.Student).Include(x => x.Project).ToListAsync();
            //var filePath = Path.Combine(_environment.ContentRootPath, "Excel", _context.Classes.FirstOrDefault(x => x.ClassId == id).ClassCode + ".xlsx");
            var classCode = _context.Classes.FirstOrDefault(x => x.ClassId == id).ClassCode;
            string filePath = Path.Combine(Path.GetTempPath(), classCode + ".xlsx");
            _excelHandler.ExportClassStudentToExcel(list, filePath);
            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", classCode + ".xlsx");
        }

        public IActionResult OnGetGetTemplate()
        {
            string filePath = Path.Combine(Path.GetTempPath(), "Template.xlsx");
            _excelHandler.ExportTemplateClassStudent(filePath);
            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template.xlsx");
        }

        public IActionResult OnPostImport()
        {
            if (FileUpload != null)
            {
                var file = Path.Combine(_environment.ContentRootPath, "Excel", string.Concat(DateTime.Now, FileUpload.FileName));
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    FileUpload.CopyTo(fileStream);
                    _excelHandler.ImportProjectFromExcel(file, Class.ClassId);
                }
            }
            return Redirect($"/ClassManager/Projects?classId={Class.ClassId}");
        }
    }
}
