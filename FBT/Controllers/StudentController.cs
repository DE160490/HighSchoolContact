using FBT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

}
