using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public DetailsModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        public User User { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            }

            if (id == null || _context.Users == null)
            {
                return Redirect("/PageNotFound");
            }

            var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return Redirect("/PageNotFound");
            }
            else 
            {
                User = user;
            }
            return Page();
        }
    }
}
