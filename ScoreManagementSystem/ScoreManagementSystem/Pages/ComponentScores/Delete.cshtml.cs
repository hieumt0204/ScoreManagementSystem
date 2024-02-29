using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.ComponentScores
{
    public class DeleteModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public DeleteModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public ComponentScore ComponentScore { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ComponentScores == null)
            {
                return Redirect("/PageNotFound");
            }
            var componentscore = await _context.ComponentScores
                .Include(c => c.Scores)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (componentscore != null)
            {
                _context.Scores.RemoveRange(componentscore.Scores);
                _context.ComponentScores.Remove(componentscore);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.ComponentScores == null)
            {
                return NotFound();
            }
            var componentscore = await _context.ComponentScores.FindAsync(id);

            if (componentscore != null)
            {
                ComponentScore = componentscore;
                _context.ComponentScores.Remove(ComponentScore);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
