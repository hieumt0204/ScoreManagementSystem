using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Subjects
{
    public class CreateModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public CreateModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            } 

            return Page();
        }

        [BindProperty]
        public Subject Subject { get; set; } = default!;
        

        public async Task<IActionResult> OnPostAsync()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            }

            if (!ModelState.IsValid || _context.Subjects == null || Subject == null)
            {
                return Page();
            }
            var subjectExistByName = _context.Subjects
                .FirstOrDefault(x => x.Name.Equals(Subject.Name));
            if (subjectExistByName != null)
            {
                ModelState.AddModelError("Subject.Name", "The subject name is already existed!");
                return Page();
            }

            Subject.CreatedBy = userId;
            Subject.CreatedAt = DateTime.Now;

            _context.Subjects.Add(Subject);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
