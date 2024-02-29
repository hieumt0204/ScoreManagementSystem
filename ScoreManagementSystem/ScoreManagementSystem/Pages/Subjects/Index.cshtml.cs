using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Subjects
{
    public class IndexModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public IndexModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        public IList<Subject> Subject { get;set; } = new List<Subject>();
        [BindProperty]
        public bool IsCurrentSubject { get; set; } = false;
        [BindProperty]
        public string? Name { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }

            if (_context.Subjects != null)
            {
                Subject = await _context.Subjects
                    .Include(s => s.CreatedByNavigation).ToListAsync();
            }

            return Page();
        }

        private void LoadSubjectOfStudent(int userId)
        {
            var classStudents = _context.ClassStudents
                .Include(x => x.Class)
                .Where(c => c.StudentId == userId)
                .ToList();

            Subject = new List<Subject>();
            foreach (var cs in classStudents)
            {
                Subject? subject = _context.Subjects.Find(cs.Class?.SubjectId);
                if (subject != null 
                    && (String.IsNullOrEmpty(Name) 
                    || subject.Name.ToLower().Contains(Name.ToLower().Trim())))
                {
                    Subject.Add(subject);
                }
            }
        }

        public IActionResult OnPost()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }

            if (IsCurrentSubject)
            {
                switch((RoleEnum)roleId)
                {
                    case RoleEnum.STUDENT:
                        LoadSubjectOfStudent((int)userId);
                        break;
                    case RoleEnum.TEACHER:
                        LoadSubjectOfTeacher((int)userId);
                        break;
                }
            }
            else
            {
                Subject = _context.Subjects
                    .Include(s => s.CreatedByNavigation)
                    .Where(s => ( String.IsNullOrEmpty(Name) ||
                        s.Name.ToLower().Contains(Name.ToLower().Trim()))
                    )
                    .ToList();
            }
            return Page();
        }

        private void LoadSubjectOfTeacher(int userId)
        {
            var subjectIds = _context.Classes
                .Where(c => c.TeacherId == userId)
                .Select(c => c.SubjectId)    
                .ToList();
            
            Subject = _context.Subjects
                .Include(s => s.CreatedByNavigation)
                .Where(s => (String.IsNullOrEmpty(Name) ||
                    s.Name.ToLower().Contains(Name.ToLower().Trim()))
                    && subjectIds.Contains(s.Id)
                )
                .ToList();
        }
    }
}
