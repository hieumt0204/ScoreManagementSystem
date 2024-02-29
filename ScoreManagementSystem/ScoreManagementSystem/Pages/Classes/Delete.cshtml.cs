using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Classes
{
    public class DeleteModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public DeleteModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        [BindProperty]
      public Class Class { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return Redirect("/PageNotFound");
            }

            var classs = await _context.Classes.FirstOrDefaultAsync(m => m.Id == id);

            if (classs == null)
            {
                return Redirect("/PageNotFound");
            }
            else 
            {
                classs.Active = !classs.Active;
                _context.Classes.Update(classs);
                _context.SaveChanges();
                return RedirectToPage("./Index");
            }
        }

        //public async Task<IActionResult> OnPostAsync(int? id)
        //{
        //    if (id == null || _context.Classes == null)
        //    {
        //        return RedirectToPage("PageNotFound");
        //    }
        //    var classs = await _context.Classes.FindAsync(id);

        //    if (classs != null)
        //    {
        //        Class = classs;
        //        _context.Classes.Remove(Class);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToPage("./Index");
        //}
    }
}
