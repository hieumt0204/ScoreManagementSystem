using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScoreManagementSystem.Models;
using ScoreManagementSystem.Services;

namespace ScoreManagementSystem.Pages.Users
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

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");

            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
           
            if (!ModelState.IsValid || _context.Users == null || User == null)
            {
                return Page();
            }
            var userInDb = _context.Users.SingleOrDefault(u => u.Email.Equals(User.Email));
            if(userInDb != null)
            {
                ModelState.AddModelError("User.Email", "Email is already existed!");
                return Page();
            }
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
            
            
            EmailServices emailServices = new EmailServices();
            await emailServices.SendAsync(new EmailMessage
            {
                To = User.Email,
                Subject = "New Account",
                Content = $"Your New Account in Score System: \n Email: {User.Email} \n Password: {User.Password}"
            });

            return RedirectToPage("./Index");
        }
    }
}
