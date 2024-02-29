using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages
{
    public class ProfileModel : PageModel
    {
        Prn221Context _context;
        public ProfileModel(Prn221Context context) 
        {
            _context = context;
        }
        [BindProperty]
        public User? UserInfo { get; set; }

        public IActionResult OnGet()
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null)
            {
                return RedirectToPage("/Home/Index");
            }
            else
            {
                UserInfo = _context.Users.SingleOrDefault(x => x.Id == userId);
                return Page();
            }
        }


        public IActionResult OnPost()
        {
            if(UserInfo != null)
            {
                User? user = _context.Users.SingleOrDefault(x => x.Email.Equals(UserInfo.Email));
                if(user != null)
                {
                    user.Name = UserInfo.Name;
                    user.Password = UserInfo.Password;
                    user.Gender = UserInfo.Gender;

                    _context.Users.Update(user);
                    _context.SaveChanges();
                    
                    UserInfo = user;
                }
                
            }

            return Page();
        }
    }
}
