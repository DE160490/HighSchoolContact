using FBT.Filter;
using FBT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.WebSockets;
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
                const int pageSize = 15; 

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


    //-------------------------------------------------------Học bạ học sinh
    public ActionResult Transcript()
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null)
        {
            return RedirectToAction("Login", "Login");
        }
        string StudentID = username.Split('$')[0];
        ViewData["Avatar"] = StudentID;

        using (var dbContext = new FbtContext())
        {
            //Lấy thông tin học sinh từ database
           var student = dbContext.Students
               .Include(s => s.Classes)
               .Include(s => s.StudentNavigation)
               .FirstOrDefault(s => s.StudentId == StudentID);
            
            if (student != null)
            {
                var schoolProfile = dbContext.SchoolProfiles.FirstOrDefault(sp => sp.StudentId == StudentID);

                if (schoolProfile != null)
                {
                    if (schoolProfile.Status == 1)
                    {
                        var transcript = (from learningOutcome in dbContext.LearningOutcomes
                                      join SchoolProfile in dbContext.SchoolProfiles on learningOutcome.SchoolProfileId equals schoolProfile.SchoolProfileId
                                      join subject in dbContext.Subjects on learningOutcome.SubjectId equals subject.SubjectId
                                      where SchoolProfile.StudentId == StudentID
                                          select new
                                      {
                                          subject.SubjectName,
                                          learningOutcome.Semester1Scores,
                                          learningOutcome.Semester2Scores,
                                          learningOutcome.FinalScores,
                                          learningOutcome.ConfirmationsOfSubjectTeacher,
                                          schoolProfile.Note,
                                      }).ToList();
                        var CountScores = transcript.Count();
                        var studentname = dbContext.PersonInformations.Where(x => x.Id == StudentID).Select(x =>x.Fullname).FirstOrDefault();
                        var scholeid = dbContext.SchoolProfiles.Where(x=>x.StudentId == StudentID).Select(x=>x.SchoolProfileId).FirstOrDefault();
                        var transcript2 = (from resultOfEvaluation in dbContext.ResultOfEvaluations
                                      join evaluateOutcome in dbContext.EvaluateEducationalOutcomes on resultOfEvaluation.SchoolProfileId equals evaluateOutcome.SchoolProfileId
                                           join SchoolProfile in dbContext.SchoolProfiles on resultOfEvaluation.SchoolProfileId equals schoolProfile.SchoolProfileId
                                           where schoolProfile.StudentId == StudentID && schoolProfile.SchoolProfileId == scholeid
                                           select new
                                      {
                                          resultOfEvaluation.Semester1Evaluation,
                                          resultOfEvaluation.Semester2Evaluation,
                                          resultOfEvaluation.FinalEvaluation,
                                          evaluateOutcome.Level,
                                          evaluateOutcome.Comment,
                                          evaluateOutcome.AbilitiesAndQualities,
                                          schoolProfile.ConfirmationsOfHomeroomTeacher,
                                          schoolProfile.ConfirmationsOfPrincipal
                                      }).ToList();
                        ViewBag.Transcript = transcript;
                        ViewBag.transcript2 = transcript2;
                        ViewBag.CountScores = CountScores;
                        ViewBag.StudentName = studentname;
                        return View();
                    }
                    else if (schoolProfile.Status == 0)
                    {
                        ViewBag.Message = "Học bạ của học sinh chưa được công bố. Nếu có vấn đề, hãy liên hệ đến giáo viên chủ nhiệm của bạn";
                    }
                }
            }
            ViewBag.Message = "Học bạ của học sinh chưa được công bố. Nếu có vấn đề, hãy liên hệ đến giáo viên chủ nhiệm của bạn";
            return View();
        }
        
    }

 
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
