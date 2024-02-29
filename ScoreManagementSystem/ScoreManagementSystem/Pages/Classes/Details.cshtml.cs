using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Classes
{
    public class DetailsModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public DetailsModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        public Class Classs { get; set; } = default!; 
        public List<User> Students { get; set; } = new List<User>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null
                || roleId == (int)RoleEnum.STUDENT || roleId == (int)RoleEnum.TEACHER)
            {
                RedirectToPage("/Home/Index");
            }

            if (id == null || _context.Classes == null)
            {
                return Redirect("/PageNotFound");
            }

            var classs = await _context.Classes.Include(c => c.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.Id == id
                    && (roleId == (int)RoleEnum.ADMIN
                        || m.Active == true)
                );
            if (classs == null)
            {
                return Redirect("/PageNotFound");
            }
            else 
            {
                Classs = classs;

                var studentIds = _context.ClassStudents
                .Where(cs => cs.ClassId == Classs.Id)
                .Select(cs => cs.StudentId)
                .ToList();

                Students = _context.Users
                    .Where(x => studentIds.Contains(x.Id))
                    .ToList();
            }
            return Page();
        }
    }
}
