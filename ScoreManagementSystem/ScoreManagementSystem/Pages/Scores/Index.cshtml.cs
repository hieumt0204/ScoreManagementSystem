using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using ScoreManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Helpers;

namespace ScoreManagementSystem.Pages.Scores
{
    public class IndexModel : PageModel
    {
        Prn221Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IndexModel(Prn221Context context, IWebHostEnvironment webHostEnvironment) 
        { 
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        [BindProperty]
        public int? SubjectId { get; set; }
        [BindProperty]
        public int? ClassId { get; set; }
        [BindProperty]
        public int? ClassToImport { get; set; }
        [BindProperty]
        public int? ComponentId { get; set; }
        [BindProperty]
        public string? SearchText { get; set; }
        [BindProperty]
        public List<ComponentScore> ComponentScores { get; set; } = new List<ComponentScore>();
        [BindProperty]
        public List<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
        [BindProperty]
        public int? roleId { get; set; }
        [BindProperty]
        public int? userId { get; set; }
        [BindProperty]
        public string Message { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please choose at least one file.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "xlsx,xls")]
        [Display(Name = "Choose file(s) to upload")]
        public IFormFile file { get; set; }
        public IActionResult OnGet()
        {
            roleId = HttpContext.Session.GetInt32("RoleId");
            userId = HttpContext.Session.GetInt32("UserId");

            if (roleId == null || userId == null || roleId == (int)RoleEnum.STUDENT)
            {
                return RedirectToPage("/Home/Index");
            }
            return Page();
        }

        public void OnPost()
        {
            roleId = HttpContext.Session.GetInt32("RoleId");
            userId = HttpContext.Session.GetInt32("UserId");
            if (ClassId != null)
            {
                Class? c = _context.Classes
                    .SingleOrDefault(c => c.Id == ClassId
                        && (roleId == (int)RoleEnum.ADMIN
                        || c.Active == true)
                    );
                if(c != null)
                {
                    //ComponentScores = _context.ComponentScores
                    //    .Where(x => x.SubjectId == c.SubjectId
                    //        && x.Active == true
                    //    )
                    //    .ToList();
                    
                    ClassStudents = _context.ClassStudents
                    .Include(x => x.Student)
                    .Where(cs => cs.ClassId == ClassId)
                    .ToList();
                }
                
            }
        }

        public JsonResult OnGetLoadClassBySubject(int subjectId)
        {
            roleId = HttpContext.Session.GetInt32("RoleId");
            userId = HttpContext.Session.GetInt32("UserId");
            var classes = _context.Classes
                .Where(c => c.SubjectId == subjectId 
                    && (c.TeacherId == userId || roleId == (int)RoleEnum.ADMIN)
                    && c.Active == true
                )
                .ToList();

            ComponentScores = _context.ComponentScores
                .Where(x => x.SubjectId == subjectId
                    && x.Active == true
                )
                .ToList();

            var result = new
            {
                classes = classes,
                ComponentScores = ComponentScores
            };

            return new JsonResult(result);
        }

        public JsonResult OnPostSaveScore([FromBody] List<Score> data)
        {
            bool isChange = false;
            if(data != null)
            {
                foreach (var item in data)
                {
                    Score? score = _context.Scores
                        .SingleOrDefault(s => s.StudentId == item.StudentId
                            && s.ComponentScoreId == item.ComponentScoreId
                        );
                    if (item.Score1 != -1)
                    {
                        if (score != null)
                        {
                            if(score.Score1 != item.Score1)
                            {
                                score.Score1 = item.Score1;
                                _context.Update(score);
                                isChange = true;
                            }
                        }
                        else
                        {
                            _context.Scores.Add(
                                new Score
                                {
                                    Score1 = item.Score1,
                                    ComponentScoreId = item.ComponentScoreId,
                                    StudentId = item.StudentId
                                }
                            );
                            isChange = true;
                        }
                    }
                    else if(score != null)
                    {
                        _context.Scores.Remove(score);
                        isChange = true;
                    }
                }
                _context.SaveChanges();
            }

            return new JsonResult(new { 
                success = true, 
                message = isChange ? "Data saved successfully!" : "No Data Change!"
            });
        }

        public async Task<IActionResult> OnPostImport()
        {
            var classs = _context.Classes
                .Include(c => c.ClassStudents)
                .SingleOrDefault(c => c.Id == ClassToImport);
            if (file != null && file.Length > 0 && classs != null)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets[0];

                        var rowCount = workSheet.Dimension.Rows;
                        var colCount = workSheet.Dimension.Columns;

                        var components = _context.ComponentScores
                            .Where(c => c.SubjectId == classs.SubjectId
                                && c.Active == true
                            )
                            .ToList();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            int? id = null;
                            try
                            {
                                id = int.Parse(workSheet.Cells[row, 1].Text);
                            }catch(Exception ex) { }
                            
                            if(id != null)
                            {
                                Dictionary<string, double> dic = new Dictionary<string, double>();
                                for (int col = 3; col <= colCount; col++)
                                {
                                    string header = workSheet.Cells[1, col].Text;
                                    double cellValue = -1;

                                    try
                                    {
                                        cellValue = double.Parse(workSheet.Cells[row, col].Text);
                                        if(cellValue < 0 || cellValue > 10)
                                        {
                                            cellValue = -1;
                                        }
                                    }
                                    catch(Exception ex) { }

                                    dic.Add(header, cellValue);
                                }
                                foreach (var item in dic)
                                {
                                    var component = components.FirstOrDefault(c => c.Name.Equals(item.Key));
                                    var classStudent = classs.ClassStudents
                                        .FirstOrDefault(c => c.StudentId == id);
                                    if (item.Value != -1 && component != null && classStudent != null)
                                    {
                                        Score? score = _context.Scores
                                            .SingleOrDefault(s => s.StudentId == classStudent.Id
                                                && s.ComponentScoreId == component.Id
                                            );
                                        if (score != null)
                                        {
                                            if (score.Score1 != item.Value)
                                            {
                                                score.Score1 = item.Value;
                                                _context.Update(score);
                                            }
                                        }
                                        else
                                        {
                                            _context.Scores.Add(
                                                new Score
                                                {
                                                    Score1 = item.Value,
                                                    ComponentScoreId = component.Id,
                                                    StudentId = classStudent.Id
                                                }
                                            );
                                        }
                                    }
                                }
                                await _context.SaveChangesAsync();
                            }
                            
                        }
                    }
                }

            }
            else
            {
                Message = "Please choose a excel file to import";
            }

            roleId = HttpContext.Session.GetInt32("RoleId");
            userId = HttpContext.Session.GetInt32("UserId");
            
            return Page();
        }

        public IActionResult OnGetExport(int? classId)
        {
            roleId = HttpContext.Session.GetInt32("RoleId");
            userId = HttpContext.Session.GetInt32("UserId");
            if (classId != null)
            {
                var classs = _context.Classes
                    .Include(c => c.ClassStudents)
                    .SingleOrDefault(c => c.Id == classId
                        && c.Active == true
                    );
                if (classs != null)
                {
                    var components = _context.ComponentScores
                        .Where(c => c.SubjectId == classs.SubjectId 
                            && c.Active == true)
                        .ToList();
                    var classStudents = classs.ClassStudents
                        .ToList();
                    if(classStudents == null || classStudents.Count == 0)
                    {
                        Message = "No student in class to export";
                        return Page();
                    }

                    byte[] excelData = CreateExcelFile(components, classStudents);

                    return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{classs.Name}_Scores.xlsx");
                }
                else
                {
                    Message = "Please choose a class to export";
                    return Page();
                }
            }
            else
            {
                Message = "Please choose a class to export";
                return Page();
            }
        }

        private string GetColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = string.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

        private byte[] CreateExcelFile(List<ComponentScore> components, List<ClassStudent> classStudents)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Scores");

                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Name";
                int columnIndex = 3;
                foreach (var item in components)
                {
                    string columnName = GetColumnName(columnIndex++);
                    worksheet.Cells[$"{columnName}1"].Value = item.Name;
                }
                worksheet.Cells[$"{GetColumnName(columnIndex++)}1"].Value = "Total";

                for (int i = 0; i < classStudents.Count; i++)
                {
                    int col = 1;
                    var student = _context.Users.Find(classStudents[i].StudentId);
                    if(student != null)
                    {
                        worksheet.Cells[i + 2, col++].Value = student.Id;
                        worksheet.Cells[i + 2, col++].Value = student.Name;
                        double? total = 0;
                        for (int j = 0; j < components.Count; j++)
                        {
                            var score = _context.Scores
                                .SingleOrDefault(s => s.ComponentScoreId == components[j].Id
                                    && s.StudentId == classStudents[i].Id
                                );
                            if (score != null)
                            {
                                worksheet.Cells[i + 2, col].Value = score.Score1 == null ? DBNull.Value : score.Score1;
                                total += score.Score1 * components[j].Percent / 100;
                            }
                            col++;
                        }
                        worksheet.Cells[i + 2, col++].Value = total==null ? DBNull.Value : total;
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    package.SaveAs(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
