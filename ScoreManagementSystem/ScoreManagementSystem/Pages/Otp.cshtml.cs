using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages
{
    public class OtpModel : PageModel
    {
        [BindProperty]
        public string Message { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string otp)
        {
            var cookieOtp = HttpContext.Request.Cookies["otp"];
            if(cookieOtp == null)
            {
                Message = "OTP is expired!";
            }
            else if (!otp.Equals(cookieOtp))
            {
                Message = "OTP is incorrect!";
            }
            else
            {
                HttpContext.Response.Cookies.Append("isCorrectOTP", "correct", new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(5)
                });
                return Redirect("/ResetPassword");
            }
            return Page();
        }
    }
}
