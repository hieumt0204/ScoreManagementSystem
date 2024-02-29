using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.ComponentScores
{
    public class EditModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public EditModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public ComponentScore ComponentScore { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            }

            if (id == null || _context.ComponentScores == null)
            {
                return Redirect("/PageNotFound");
            }

            var componentscore =  await _context.ComponentScores.FirstOrDefaultAsync(m => m.Id == id);
            if (componentscore == null)
            {
                return Redirect("/PageNotFound");
            }
            
            ComponentScore = componentscore;
            LoadReferenceData();

            return Page();
        }

        private void LoadReferenceData()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadReferenceData();
            if (!ModelState.IsValid)
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
                    && c.Id != ComponentScore.Id
                );
            if (component != null)
            {
                ModelState.AddModelError("ComponentScore.Name", "The name component " +
                    "score of the this subject is existed!");
                return Page();
            }

            var totalPercent = _context.ComponentScores
               .Where(c => c.SubjectId == ComponentScore.SubjectId 
                    && c.Id != ComponentScore.Id)
               .Select(x => x.Percent)
               .ToList().Sum();

            if(totalPercent + ComponentScore.Percent > 100)
            {
                ModelState.AddModelError("ComponentScore.Percent", "Percent of this component" +
                    " score must be <= " + (100 - totalPercent) + "!");
                return Page();
            }

            _context.Attach(ComponentScore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComponentScoreExists(ComponentScore.Id))
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

        private bool ComponentScoreExists(int id)
        {
          return (_context.ComponentScores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
