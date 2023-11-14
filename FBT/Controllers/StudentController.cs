using FBT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBT.Controllers;

public class StudentController : Controller
{
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

    public ActionResult StudentScore(string studentId)
    {

        return View();
    }

}
