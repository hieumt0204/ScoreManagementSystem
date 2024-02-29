using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Scores
{
    public class StudentModel : PageModel
    {
        Prn221Context _context;

        public StudentModel(Prn221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public List<Score> Scores { get; set; } = new List<Score>();
        [BindProperty]
        public List<Subject> subjects { get; set; } = new List<Subject>();
        
        [BindProperty]
        public List<ComponentScore> components { get; set;} = new List<ComponentScore>();
        [BindProperty]
        public int? SubjectId { get; set; }
        [BindProperty]
        public int? userId { get; set; }

        public IActionResult OnGet()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.STUDENT)
            {
                return RedirectToPage("/Home/Index");
            }

            LoadReferenceData();

            return Page();
        }

        private void LoadReferenceData()
        {
            var classStudents = _context.ClassStudents
                .Include(x => x.Class)
                .Where(c => c.StudentId == userId)
                .ToList();


            foreach (var cs in classStudents)
            {

                Subject? subject = _context.Subjects.Find(cs.Class?.SubjectId);
                if (subject != null && subject.Active == true)
                {
                    subjects.Add(subject);
                }
            }
        }

        public IActionResult OnPost()
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (SubjectId != null)
            {
                components = _context.ComponentScores
                    .Where(c => c.SubjectId == SubjectId
                        && c.Active == true
                    )
                    .OrderBy(x => x.Percent)
                    .ToList();
            }
            LoadReferenceData();
            return Page();
        }
    }
}
