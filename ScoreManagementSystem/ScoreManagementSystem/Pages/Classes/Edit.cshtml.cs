using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScoreManagementSystem.Models;

namespace ScoreManagementSystem.Pages.Classes
{
    public class EditModel : PageModel
    {
        private readonly ScoreManagementSystem.Models.Prn221Context _context;

        public EditModel(ScoreManagementSystem.Models.Prn221Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Class Class { get; set; } = default!;
        [BindProperty]
        public int? OldSubjectId { get; set; }

        [BindProperty]
        public List<int> StudentIdsInClass { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null
                || roleId == (int)RoleEnum.STUDENT || roleId == (int)RoleEnum.TEACHER)
            {
                return RedirectToPage("/Home/Index");
            }

            if (id == null || _context.Classes == null)
            {
                return Redirect("/PageNotFound");
            }

            var classs =  await _context.Classes.Include(c => c.Subject).FirstOrDefaultAsync(m => m.Id == id);
            if (classs == null)
            {
                return Redirect("/PageNotFound");
            }
            Class = classs;
            OldSubjectId = Class.SubjectId;
            LoadReferenceData();
            return Page();
        }

        private void LoadReferenceData()
        {
            var studentIds = _context.ClassStudents
                .Where(cs => cs.ClassId == Class.Id)
                .Select(cs => cs.StudentId)    
                .ToList();
            
            StudentIdsInClass = _context.Users
                .Where(x => studentIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToList();
            
            ViewData["TeacherId"] = new SelectList(_context.Users
                .Where(u => u.RoleId == (int)RoleEnum.TEACHER), "Id", "Name");

            ViewData["SubjectId"] = new SelectList(_context.Subjects
                .Where(s => s.Active == true), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync(int[]? studentId)
        {
            int? roleId = HttpContext.Session.GetInt32("RoleId");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null
                || roleId == (int)RoleEnum.STUDENT || roleId == (int)RoleEnum.TEACHER)
            {
                return RedirectToPage("/Home/Index");
            }

            LoadReferenceData();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var classExistByName = _context.Classes
                .FirstOrDefault(x => x.Name.Equals(Class.Name)
                    && x.Id != Class.Id
                );
            if (classExistByName != null)
            {
                ModelState.AddModelError("Class.Name", "The class name is already existed!");
                return Page();
            }

            _context.Attach(Class).State = EntityState.Modified;

            if(OldSubjectId != Class.SubjectId)
            {
                studentId = null;
                var studentsInCurrentClass = _context.ClassStudents
                    .Where(x => x.ClassId == Class.Id)
                    .ToList();
                _context.ClassStudents.RemoveRange(studentsInCurrentClass);
            }

            try
            {
                if(studentId != null)
                {
                    //add new
                    List<ClassStudent> classStudents = new List<ClassStudent>();
                    foreach (var item in studentId)
                    {
                        var Student = _context.Users.FirstOrDefault(x => x.Id == item 
                            && x.RoleId == (int)RoleEnum.STUDENT);
                        
                        if (Student != null)
                        {
                            if (!StudentIdsInClass.Contains(Student.Id))
                            {
                                classStudents.Add(
                                    new ClassStudent
                                    {
                                        ClassId = Class.Id,
                                        StudentId = item,
                                        JoinDate = DateTime.Now
                                    }        
                                );
                            }
                        }
                    }

                    if(classStudents.Count > 0)
                    {
                        _context.ClassStudents.AddRange(classStudents);
                    }

                    //delete
                    classStudents = new List<ClassStudent>();
                    foreach (var id in StudentIdsInClass)
                    {
                        if (!studentId.Contains(id))
                        {
                            ClassStudent? cs = _context.ClassStudents
                                .SingleOrDefault(c => c.ClassId == Class.Id
                                    && c.StudentId == id);
                            if(cs != null)
                                classStudents.Add(cs);
                        }
                    }

                    if (classStudents.Count > 0)
                    {
                        _context.ClassStudents.RemoveRange(classStudents);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(Class.Id))
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

        private bool ClassExists(int id)
        {
          return (_context.Classes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
