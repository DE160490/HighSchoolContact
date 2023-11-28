using FBT.Filter;
using FBT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Globalization;
using X.PagedList;

namespace FBT.Controllers;
public class StudentController : Controller
{
//// ====================================== Student Controller ======================================
    // Get
    public ActionResult Index()
    {
        var username = HttpContext.Session.GetString("Username");
        if(username == null)
        {
            return RedirectToAction("Login", "Login");
        }
        string StudentID = username.Split('$')[0];
        ViewData["Avatar"] = StudentID;
        Student? student = GetStudentData(StudentID);
        return View(student);
    }

    // Get Student Database
    private Student? GetStudentData(string studentId)
    {
        try { 
            using (var dbContext = new FbtContext())
            {
                var student = dbContext.Students
                    .Include(s => s.StudentNavigation)
                    .Include(s => s.Parents)
                    .ThenInclude(s => s.ParentNavigation)
                    .FirstOrDefault(s => s.StudentId == studentId);
                if(student != null)
                {
                    return student;
                }

            }
        }catch(SqlException e) {
            Console.WriteLine(e.Message);
        }
        return null;
    }

    // Get Score of Student
    public ActionResult StudentScore(int? page)
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null)
        {
            return RedirectToAction("Login", "Login");
        }
        string StudentID = username.Split('$')[0];
        try { 
            using (var dbContext = new FbtContext())
            {
                const int pageSize = 10; 

                var scores = dbContext.Scores
                    .Include(s=> s.Subject)
                    .Where(s => s.StudentId == StudentID && s.Semester == (page ?? 1))
                    .OrderBy(s => s.Semester)
                    .ToPagedList(1, pageSize);

                ViewBag.StudentId = StudentID;
                ViewBag.CurrentSemester = page ?? 1;
                ViewData["Avatar"] = StudentID;
                return View(scores);
            }
        }catch(SqlException e) { 
            Console.WriteLine(e.Message); 
            return View();
        }
    }

//// ====================================== Schedule ======================================
    // Get
    public ActionResult TimeTable()
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null) {
            return RedirectToAction("Login", "Login");
        }

        string StudentID = username.Split('$')[0];
        ViewData["Avatar"] = StudentID;
        try { 
            using (var dbContext = new FbtContext())
            {
                var student = dbContext.Students
                    .Include(s => s.Classes)
                    .Include(s => s.StudentNavigation)
                    .FirstOrDefault(s => s.StudentId == StudentID);

                if (student != null) {
                    if (student != null) {

                        var classId = student.Classes.FirstOrDefault()?.ClassId;

                        var now = DateTime.Now;
                        DateTime weekBegins = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday).Date;
                        DateTime weekEnds = weekBegins.AddDays(7).AddSeconds(-1);

                        var schedules = dbContext.Schedules
                            .Where(s => s.ClassId == classId && s.WeekBegins >= weekBegins && s.WeekEnds <= weekEnds)
                            .Include(s => s.Teacher.TeacherNavigation)
                            .Include(s => s.Subject)
                            .ToList();

                        ViewBag.StudentName = student.StudentNavigation.Fullname;
                        ViewBag.WeekBegins = weekBegins;
                        ViewBag.WeekEnds = weekEnds;

                        ViewBag.StudentId = StudentID;
                        return View(schedules);
                    }
                }
            }
        }catch(SqlException e) { 
            Console.WriteLine(e.Message); 
            return View("ErrorSystem");
        }
        return View();
    }

    // Post
    [HttpPost]
    public ActionResult TimeTable(string weekBegins, string weekEnds)
    {
        DateTime? weekBeginsDate = null;
        DateTime? weekEndsDate = null;
        var username = HttpContext.Session.GetString("Username");
        if (username == null)
        {
            return RedirectToAction("Login", "Login");
        }
        string StudentID = username.Split('$')[0];
        ViewData["Avatar"] = StudentID;

        if (DateTime.TryParseExact(weekBegins, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedWeekBegins))
        {
            weekBeginsDate = parsedWeekBegins;
        }

        if (DateTime.TryParseExact(weekEnds, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedWeekEnds))
        {
            weekEndsDate = parsedWeekEnds;
        }
        try { 
            using (var dbContext = new FbtContext())
            {
                var student = dbContext.Students
                    .Include(s => s.Classes)
                    .Include(s => s.StudentNavigation)
                    .FirstOrDefault(s => s.StudentId == StudentID);

                if (student != null)
                {
                    var classId = student.Classes.FirstOrDefault()?.ClassId;

                    if (classId != null)
                    {
                        var schedules = dbContext.Schedules
                            .Where(s => s.ClassId == classId && s.WeekBegins >= weekBeginsDate && s.WeekEnds <= weekEndsDate)
                            .Include(s => s.Teacher.TeacherNavigation)
                            .Include(s => s.Subject)
                            .ToList();

                        ViewBag.StudentName = student.StudentNavigation.Fullname;
                        ViewBag.WeekBegins = weekBeginsDate;
                        ViewBag.WeekEnds = weekEndsDate;
                        ViewBag.StudentId = StudentID;
                        return View(schedules);
                    }
                }
            }
        }catch(SqlException e) { 
            Console.WriteLine(e.Message);
            return View("ErrorSystem");
        }
        return View();
    }

}
