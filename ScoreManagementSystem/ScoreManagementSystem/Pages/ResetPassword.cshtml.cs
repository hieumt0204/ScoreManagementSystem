using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages
{
    public class ResetPasswordModel : PageModel
    {
        Prn221Context _context;

        public ResetPasswordModel(Prn221Context context)
        {
            _context = context;
        }


        [BindProperty]
        public string Message { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }

        public IActionResult OnGet()
        {
            var cookieIsCorrectOtp = HttpContext.Request.Cookies["isCorrectOTP"];
            if(cookieIsCorrectOtp == null || !cookieIsCorrectOtp.Equals("correct"))
            {
                Message = "You need to confirm OTP to reset password!";
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            var cookieIsCorrectOtp = HttpContext.Request.Cookies["isCorrectOTP"];
            if (cookieIsCorrectOtp == null || !cookieIsCorrectOtp.Equals("correct"))
            {
                Message = "You need to confirm OTP to reset password!";
            }

            var email = HttpContext.Request.Cookies["emailForgotPassword"];
            if(email == null)
            {
                Message = "Time Reset Password is expired! You need to resend email!";
                return Page();
            }
            
            if (NewPassword.Equals(ConfirmPassword))
            {
                var user = _context.Users.FirstOrDefault(x => x.Email.Equals(email));
                user.Password = NewPassword;
                _context.Users.Update(user);
                _context.SaveChanges();
                return Redirect("/Login");
            }
            else
            {
                Message = "New Password must be equals to Confirm Password!";
                return Page();
            }
        }
    }
}
