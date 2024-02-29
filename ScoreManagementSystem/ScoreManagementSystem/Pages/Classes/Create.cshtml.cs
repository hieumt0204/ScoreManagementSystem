using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Classes
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
            LoadReferenceData();
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null
                || roleId == (int)RoleEnum.STUDENT || roleId == (int)RoleEnum.TEACHER)
            {
                return RedirectToPage("/Home/Index");
            }

            return Page();
        }

        private void LoadReferenceData()
        {
            ViewData["TeacherId"] = new SelectList(_context.Users
                .Where(u => u.RoleId==(int)RoleEnum.TEACHER), "Id", "Name");

            ViewData["SubjectId"] = new SelectList(_context.Subjects
                .Where(s => s.Active==true), "Id", "Name");
        }

        [BindProperty]
        public Class Class { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            LoadReferenceData();
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null 
                || roleId == (int)RoleEnum.STUDENT || roleId == (int)RoleEnum.TEACHER)
            {
                return RedirectToPage("/Home/Index");
            }

            if (!ModelState.IsValid || _context.Classes == null || Class == null)
            {
                return Page();
            }
            var classExistByName = _context.Classes
                .FirstOrDefault(x => x.Name.Equals(Class.Name));
            if (classExistByName != null)
            {
                ModelState.AddModelError("Class.Name", "The class name is already existed!");
                return Page();
            }

            Class.CreatedBy = userId;
            Class.CreatedAt = DateTime.Now;
            
            _context.Classes.Add(Class);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
