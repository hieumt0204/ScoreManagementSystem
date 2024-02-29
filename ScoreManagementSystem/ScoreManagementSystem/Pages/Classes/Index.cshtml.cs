using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Classes
{
    public class IndexModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public IndexModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        public IList<Class> Class { get;set; } = new List<Class>();

        [BindProperty]
        public bool IsCurrentClass { get; set; } = false;
        [BindProperty]
        public string? Name { get; set; }
        [BindProperty]
        public int? SubjectId { get; set; } = -1;
        [BindProperty]
        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public async Task<IActionResult> OnGetAsync()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if(roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }

            if (_context.Classes != null)
            {
                Subjects = _context.Subjects
                    .Where(x => x.Active == true)
                    .ToList();
                Class = await _context.Classes
                    .Include(c => c.CreatedByNavigation)
                    .Include(c => c.Subject)
                    .Include(c => c.Teacher)
                    .Where(c => roleId == (int)RoleEnum.ADMIN 
                        || c.Active == true
                    )
                    .ToListAsync();
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }
            Subjects = _context.Subjects
                .Where(x => x.Active == true)
                .ToList();

            if (IsCurrentClass)
            {
                switch ((RoleEnum)roleId)
                {
                    case RoleEnum.TEACHER:
                        Class = await _context.Classes
                            .Include(c => c.CreatedByNavigation)
                            .Include(c => c.Subject)
                            .Include(c => c.Teacher)
                            .Where(x => x.TeacherId == userId
                                && (String.IsNullOrEmpty(Name) 
                                    || x.Name.ToLower().Contains(Name.ToLower().Trim()))
                                && x.Active == true
                                && (SubjectId == -1 || x.Subject.Id == SubjectId)
                            )
                            .ToListAsync();
                        break;
                    case RoleEnum.STUDENT:
                        Class = await _context.Classes
                            .Include(c => c.CreatedByNavigation)
                            .Include(c => c.Subject)
                            .Include(c => c.Teacher)
                            .Include(c => c.ClassStudents)
                            .Where(c =>
                                (c.ClassStudents.Where(cs => cs.StudentId == userId)
                                    .Count() > 0)
                                && (String.IsNullOrEmpty(Name)
                                    || c.Name.ToLower().Contains(Name.ToLower().Trim()))
                                && c.Active == true
                                && (SubjectId == -1 || c.Subject.Id == SubjectId)
                            )
                            .ToListAsync();
                        break;
                }
            }
            else
            {
                Class = await _context.Classes
                    .Include(c => c.CreatedByNavigation)
                    .Include(c => c.Subject)
                    .Include(c => c.Teacher)
                    .Where(c => (String.IsNullOrEmpty(Name)
                        || c.Name.ToLower().Contains(Name.ToLower().Trim()))
                        && (SubjectId == -1 || c.Subject.Id == SubjectId)
                    )
                    .ToListAsync();
            }

            return Page();
        }
    }
}
