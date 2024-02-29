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
    public class DetailsModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public DetailsModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

      public ComponentScore ComponentScore { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }

            if (id == null || _context.ComponentScores == null)
            {
                return Redirect("/PageNotFound");
            }

            var componentscore = await _context.ComponentScores
                .FirstOrDefaultAsync(m => m.Id == id
                    && (roleId == (int)RoleEnum.ADMIN
                        || m.Active == true
                    )
                );
            if (componentscore == null)
            {
                return Redirect("/PageNotFound");
            }
            else 
            {
                ComponentScore = componentscore;
            }
            return Page();
        }
    }
}
