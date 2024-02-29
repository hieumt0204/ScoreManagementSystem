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
    public class IndexModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public IndexModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        public IList<ComponentScore> ComponentScore { get;set; } = default!;

        [BindProperty]
        public int? SubjectId { get; set; } = -1;
        [BindProperty]
        public string ComponentName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }

            if (_context.ComponentScores != null)
            {
                ComponentScore = await _context.ComponentScores
                    .Include(c => c.Subject)
                    .Where(c => roleId == (int)RoleEnum.ADMIN  
                        || c.Active == true
                    )
                    .ToListAsync();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }

            if (SubjectId == null)
            {
                return Redirect("/PageNotFound");
            }
            if (_context.ComponentScores != null)
            {
                ComponentScore = _context.ComponentScores
                    .Where(c => (SubjectId == -1 || c.SubjectId == SubjectId)
                        && (String.IsNullOrEmpty(ComponentName) 
                        || c.Name.ToLower().Contains(ComponentName.Trim().ToLower()))
                        && (roleId == (int)RoleEnum.ADMIN
                        || c.Active == true)
                    )   
                    .Include(c => c.Subject).ToList();
            }
            return Page();
        }
    }
}
