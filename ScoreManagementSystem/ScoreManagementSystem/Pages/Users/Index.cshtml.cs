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
    public class IndexModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public IndexModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = new List<User>();
        [BindProperty]
        public int? RoleId { get; set; }
        [BindProperty]
        public string? Name { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int) RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            }



            if (_context.Users != null)
            {
                User = await _context.Users
                .Include(u => u.Role).ToListAsync();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId != (int)RoleEnum.ADMIN)
            {
                return RedirectToPage("/Home/Index");
            }

            if (_context.Users != null)
            {
                User = _context.Users
                    .Where(u => (RoleId == null || u.RoleId == RoleId)
                        && (String.IsNullOrEmpty(Name) || u.Name.ToLower().Contains(Name.ToLower().Trim()))
                    )
                    .Include(u => u.Role).ToList();
            }

            return Page();
        }
    }
}
