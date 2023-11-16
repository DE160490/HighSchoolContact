using FBT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using X.PagedList;

namespace FBT.Controllers;

public class StudentController : Controller
{
    // GET: StudentController
    public ActionResult Index()
    {
        var username = HttpContext.Session.GetString("Username");
        if(username == null)
        {
            return RedirectToAction("Login", "Login");
        }
        string StudentID = username.Split('$')[0];
        ViewData["Avatar"] = StudentID;
        Student student = GetStudentData(StudentID);
        return View(student);
    }

    private Student GetStudentData(string studentId)
    {
        // Replace ApplicationDbContext with your actual DbContext class
        using (var dbContext = new FbtContext())
        {
            // Assuming you have a DbSet<Student> in your DbContext
            // Include related PersonInformation data
            var student = dbContext.Students
                .Include(s => s.StudentNavigation)
                .Include(s => s.Parents)
                .ThenInclude(s => s.ParentNavigation)
                .FirstOrDefault(s => s.StudentId == studentId);

            return student;
        }
    }

    public ActionResult StudentScore(int? page)
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null)
        {
            return RedirectToAction("Login", "Login");
        }
        string StudentID = username.Split('$')[0];

        using (var dbContext = new FbtContext())
        {
            const int pageSize = 10; // You can adjust this based on your requirement

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

    }



    //-------------------------------------------------------Thoi Khoa Bieu
    public ActionResult TimeTable(string studentId)
    {

        using (var dbContext = new FbtContext())
        {
            // Retrieve the student with related classes
            var student = dbContext.Students
                .Include(s => s.Classes)
                .Include(s => s.StudentNavigation)
                .FirstOrDefault(s => s.StudentId == studentId);

            // Check if the student exists
            if (student != null)
            {
                // Assuming a student is associated with only one class


                if (student != null)
                {

                    var classId = student.Classes.FirstOrDefault()?.ClassId;

                    var now = DateTime.Now;
                    DateTime weekBegins = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday).Date;
                    DateTime weekEnds = weekBegins.AddDays(7).AddSeconds(-1);
                    // Retrieve the schedules for the classId

                    var schedules = dbContext.Schedules
                        .Where(s => s.ClassId == classId && s.WeekBegins >= weekBegins && s.WeekEnds <= weekEnds)
                        .Include(s => s.Teacher.TeacherNavigation)
                        .Include(s => s.Subject)
                        .ToList();


                    ViewBag.StudentName = student.StudentNavigation.Fullname;
                    ViewBag.WeekBegins = weekBegins;
                    ViewBag.WeekEnds = weekEnds;

                    ViewBag.StudentId = studentId;
                    return View(schedules);
                }
            }

            // Handle the case where the student or class is not found
            return View("Error"); // You can create an error view or redirect to another action
        }
    }


    [HttpPost]
    public ActionResult TimeTable(string studentId, string weekBegins, string weekEnds)
    {
        DateTime? weekBeginsDate = null;
        DateTime? weekEndsDate = null;

        if (DateTime.TryParseExact(weekBegins, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedWeekBegins))
        {
            weekBeginsDate = parsedWeekBegins;
        }

        if (DateTime.TryParseExact(weekEnds, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedWeekEnds))
        {
            weekEndsDate = parsedWeekEnds;
        }

        using (var dbContext = new FbtContext())
        {
            var student = dbContext.Students
                .Include(s => s.Classes)
                .Include(s => s.StudentNavigation)
                .FirstOrDefault(s => s.StudentId == studentId);

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
                    ViewBag.StudentId = studentId;
                    return View(schedules);
                }
            }

            return View("Error");
        }
    }

}
