using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Subjects
{
    public class DeleteModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public DeleteModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        [BindProperty]
      public Subject Subject { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            }

            if (id == null || _context.Subjects == null)
            {
                return Redirect("/PageNotFound");
            }

            var subject = await _context.Subjects.FirstOrDefaultAsync(m => m.Id == id);

            if (subject == null)
            {
                return Redirect("/PageNotFound");
            }
            else 
            {
                subject.Active = !subject.Active;
                _context.Subjects.Update(subject);
                _context.SaveChanges();
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }
            var subject = await _context.Subjects.FindAsync(id);

            if (subject != null)
            {
                Subject = subject;
                _context.Subjects.Remove(Subject);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
