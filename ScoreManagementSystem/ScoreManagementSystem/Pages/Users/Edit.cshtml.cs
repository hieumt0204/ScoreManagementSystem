using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public EditModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        [BindProperty]
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

            var user =  await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return Redirect("/PageNotFound");
            }
            User = user;
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
           
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
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

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
