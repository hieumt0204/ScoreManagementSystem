using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScoreManagementSystem.Models;
using ScoreManagementSystem.Services;
using System.Text;

namespace ScoreManagementSystem.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        Prn221Context _context;
        public ForgotPasswordModel(Prn221Context context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        [BindProperty] 
        public string Message { get; set; }

        public async Task<IActionResult> OnPost(string? email)
        {
            User? user = _context.Users.SingleOrDefault(u => u.Email == email);
            if(email == null || user == null)
            {
                Message = "Email is not exist!";
                return Page();
            }
            else
            {
                EmailServices emailServices = new EmailServices();
                string otp = GenerateRandomString(10);
                await emailServices.SendAsync(new EmailMessage
                {
                    To = user.Email,
                    Subject = "Send OTP",
                    Content = $"Your Otp to reset password is: {otp}"
                });
                
                HttpContext.Response.Cookies.Append("otp", otp, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(5)
                });
                HttpContext.Response.Cookies.Append("emailForgotPassword", user.Email, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(5)
                });
                return Redirect("/Otp");
            }
        }

        private string GenerateRandomString(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(characters.Length);
                result.Append(characters[randomIndex]);
            }

            return result.ToString();
        }
    }
}
