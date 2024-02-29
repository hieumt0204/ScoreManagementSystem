using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Subjects
{
    public class EditModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public EditModel(ScoreManagementSystem.Models.Prn221Context context)
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

            var subject =  await _context.Subjects.FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return Redirect("/PageNotFound");
            }

            Subject = subject;
           
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var subjectExistByName = _context.Subjects
                .FirstOrDefault(x => x.Name.Equals(Subject.Name)
                    && x.Id != Subject.Id
                );
            if(subjectExistByName != null)
            {
                ModelState.AddModelError("Subject.Name", "The subject name is already existed!");
                return Page();
            }

            _context.Attach(Subject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(Subject.Id))
                {
                    return Redirect("/PageNotFound");
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
          return (_context.Subjects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
