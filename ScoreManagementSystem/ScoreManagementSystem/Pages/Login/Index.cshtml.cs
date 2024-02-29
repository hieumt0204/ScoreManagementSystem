using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using ScoreManagementSystem.Models;
using ScoreManagementSystem.Services;

namespace ScoreManagementSystem.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly Prn221Context _context;

        public IndexModel(Prn221Context context)
        {
            _context = context;
        }
        [BindProperty]
        public User User { get; set; }
        [BindProperty]
        public User RegisterUser { get; set; }
        [BindProperty]
        public string? ConfirmPassword { get; set; }
        [BindProperty]
        public string? Message { get; set; }
        public IActionResult OnGet()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != null)
            {
                if (roleId == (int)RoleEnum.ADMIN)
                    return Redirect("Users");

                return Redirect("Classes");
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPostLogin()
        {
            if (_context.Users == null || User.Email == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(m => m.Email.Equals(User.Email));
            if (user == null || User.Password == null || !User.Password.Equals(user.Password))
            {
                Message = "Email or Password is incorrect!";
            }
            else if(user.Active == false)
            {
                Message = "Your Account is InActive!";
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                if(user.RoleId != null)
                    HttpContext.Session.SetInt32("RoleId", (int)user.RoleId);
                
                if (user.RoleId == (int)RoleEnum.ADMIN)
                    return Redirect("Users");
                    
                return Redirect("Classes");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRegister()
        {
            if (_context.Users == null || RegisterUser.Email == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Email.Equals(RegisterUser.Email));
            if(user != null)
            {
                Message = "Email is already existed!";
            }
            else if (RegisterUser.Password == null || !RegisterUser.Password.Equals(ConfirmPassword))
            {
                Message = "Confirm Password is not match with Password!";
            }
            else
            {
                RegisterUser.RoleId = (int) RoleEnum.STUDENT;
                RegisterUser.Active = true;

                _context.Users.Add(RegisterUser);
                _context.SaveChanges();

                return Redirect("/Login");
            }
            return Page();
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("RoleId");
            return Redirect("/Home/Index");
        }

    }
}
