using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.ComponentScores
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

            LoadReferenceData();
            return Page();
        }


        private void LoadReferenceData()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
        }

        [BindProperty]
        public ComponentScore ComponentScore { get; set; } = default!;
        

        public async Task<IActionResult> OnPostAsync()
        {
            LoadReferenceData();
            if (!ModelState.IsValid || _context.ComponentScores == null 
                || ComponentScore == null)
            {
                return Page();
            }
            if (ComponentScore.Percent < 0 || ComponentScore.Percent > 100)
            {
                ModelState.AddModelError("ComponentScore.Percent", "Percent of this component" +
                    " must be in range 0 to 100!");
                return Page();
            }
            
            var component = _context.ComponentScores
                .FirstOrDefault(c => c.Name.Equals(ComponentScore.Name.Trim())
                    && c.SubjectId == ComponentScore.SubjectId
                );
            if (component != null)
            {
                ModelState.AddModelError("ComponentScore.Name", "The name component " +
                    "score of the this subject is existed!");
                return Page();
            }

                var totalPercent = _context.ComponentScores
                .Where(c => c.SubjectId == ComponentScore.SubjectId)
                .Select(x => x.Percent)
                .ToList().Sum();

            if(totalPercent == 100)
            {
                ModelState.AddModelError("ComponentScore.Percent", "Percent of this component" +
                    " score is fully 100%!");
                return Page();
            }
            else if(totalPercent+ComponentScore.Percent > 100)
            {
                ModelState.AddModelError("ComponentScore.Percent", "Percent of this component" +
                    " score must be <= " + (100 - totalPercent) + "!");
                return Page();
            }

            _context.ComponentScores.Add(ComponentScore);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
