using FBT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace FBT.Controllers;

public class StudentController : Controller
{
    //private readonly FbtContext _dbContext; // Replace FbtContext with your actual DbContext class

    //public StudentController(FbtContext dbContext)
    //{
    //    _dbContext = dbContext;
    //}

    // GET: StudentController
    public ActionResult Index(string studentId)
    {
        Student student = GetStudentData(studentId);
        return View(student);
        //return View();
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

    public ActionResult StudentScore(string studentId, int? page)
    {
        using (var dbContext = new FbtContext())
        {
            const int pageSize = 10; // You can adjust this based on your requirement

            var scores = dbContext.Scores
                .Include(s=> s.Subject)
                .Where(s => s.StudentId == studentId && s.Semester == (page ?? 1))
                .OrderBy(s => s.Semester)
                .ToPagedList(1, pageSize);

            ViewBag.StudentId = studentId;
            ViewBag.CurrentSemester = page ?? 1;

            return View(scores);
        }

    }

}
