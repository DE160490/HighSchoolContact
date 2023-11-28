using FBT.Models;
using FBT.Reponsitory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System;
using System.Security.Principal;
using X.PagedList;

namespace FBT.Controllers;

public class AdminController : Controller
{
//// ====================================== Manager Account ======================================
    // Get: AdminController
    public ActionResult Index()
    {
        using (var dbContext = new FbtContext())
        {
            List<Account> objAccountList = dbContext.Accounts
                .Include(a => a.AccountNavigation)
                .Where(a => a.Role != 3)
                .ToList();
            return View(objAccountList);
        }
    }

//  --------------------------------------- Create ---------------------------------------
    // Get
    public IActionResult Create()
    {
        return View();
    }
    
    // Post
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Fullname,Gender,Dob,PlaceOfBirth,PlaceOfResidence,Phone,Email,Ethnic,Religion")] PersonInformation personInformation, string Password, string Role, string? Job, string? MainExpertise, string? Position, string? StudentID)
    {
        using (var dbContext = new FbtContext())
        {
            ModelState.Remove("Account");
            ModelState.Remove("Id");

            var lastId = dbContext.PersonInformations
                .Where(p => p.Id.StartsWith(Role))
                .OrderByDescending(p => p.Id)
                .Select(p => p.Id)
                .FirstOrDefault();

            if (lastId == null)
            {
                personInformation.Id = Role + "0000001";
            }
            else
            {
                var numberPart = int.Parse(lastId.Substring(3));
                var nextNumberPart = (numberPart + 1).ToString("D7");
                personInformation.Id = Role + nextNumberPart;
            }

            if (dbContext.PersonInformations.Any(p => p.Id == personInformation.Id))
            {
                ModelState.AddModelError("Id", "A person with this Id already exists.");
            }

            if (ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToArray();

                dbContext.PersonInformations.Add(personInformation);
                dbContext.SaveChanges();

                if (personInformation.Account == null)
                {
                    personInformation.Account = new Account();
                }

                var account = personInformation.Account;
                account.AccountId = personInformation.Id;
                account.Password = Password;

                string idPrefix = personInformation.Id.Substring(0, 3);
                if (idPrefix == "HSS")
                {
                    account.Role = 0;
                }
                else if (idPrefix == "HSE")
                {
                    account.Role = 1;
                }
                else if (idPrefix == "HSP")
                {
                    account.Role = 2;
                }
                else
                {
                    account.Role = -1;
                }

                dbContext.Accounts.Add(account);

                if (Role == "HSS")
                {
                    var student = new Student { StudentId = personInformation.Id };
                    dbContext.Students.Add(student);
                }
                else if (Role == "HSE")
                {
                    var teacher = new Teacher { TeacherId = personInformation.Id, MainExpertise = MainExpertise, Position = Position ?? "Giáo Viên" };
                    dbContext.Teachers.Add(teacher);
                }
                else if (Role == "HSP")
                {
                    var parent = new Parent { ParentId = personInformation.Id, Job = Job, StudentId = StudentID };
                    dbContext.Parents.Add(parent);
                }

                dbContext.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }

            return View(personInformation);
        }
    }

//  --------------------------------------- Edit ---------------------------------------
    // Get: Admin/Edit/5
    public IActionResult Edit(string id)
    {
        using (var dbContext = new FbtContext())
        {
            var personInformation = dbContext.PersonInformations
                .Include(p => p.Account)
                .SingleOrDefault(p => p.Id == id);

            if (personInformation == null)
            {
                return NotFound();
            }

            ViewData["Password"] = personInformation.Account?.Password;

            return View("Edit", personInformation);
        }
    }

    // Post
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, [Bind("Id,Fullname,Gender,Dob,PlaceOfBirth,PlaceOfResidence,Phone,Email,Ethnic,Religion")] PersonInformation personInformation, string Password)
    {
        using (var dbContext = new FbtContext())
        {
            ModelState.Remove("Account");

            if (id != personInformation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPersonInformation = dbContext.PersonInformations
                        .Include(p => p.Account)
                        .FirstOrDefault(p => p.Id == id);

                    if (existingPersonInformation == null)
                    {
                        return NotFound();
                    }


                    personInformation.Id = id;
                    existingPersonInformation.Id = personInformation.Id;
                    existingPersonInformation.Fullname = personInformation.Fullname;
                    existingPersonInformation.Gender = personInformation.Gender;
                    existingPersonInformation.Dob = personInformation.Dob;
                    existingPersonInformation.PlaceOfBirth = personInformation.PlaceOfBirth;
                    existingPersonInformation.PlaceOfResidence = personInformation.PlaceOfResidence;
                    existingPersonInformation.Phone = personInformation.Phone;
                    existingPersonInformation.Email = personInformation.Email;
                    existingPersonInformation.Ethnic = personInformation.Ethnic;
                    existingPersonInformation.Religion = personInformation.Religion;


                    existingPersonInformation.Account.Password = Password;

                    dbContext.Update(existingPersonInformation);
                    dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!dbContext.PersonInformations.Any(p => p.Id == personInformation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Admin");
            }
            return View(personInformation);
        }
    }

//  --------------------------------------- Detail ---------------------------------------
    // Get
    public IActionResult Details(string id)
    {
        using (var dbContext = new FbtContext())
        {
            // Lấy Data trong PersonInformations Table dựa vào AccountId
            var personInformation = dbContext.PersonInformations
                .Include(p => p.Account)
                .FirstOrDefault(p => p.Id == id);

            if (personInformation == null)
            {
                return NotFound();
            }

            return View(personInformation);
        }
    }

//  --------------------------------------- Delete ---------------------------------------
    // Get
    public IActionResult Delete(string id)
    {
        using (var dbContext = new FbtContext())
        {
            var personInformation = dbContext.PersonInformations.Find(id);
            if (personInformation == null)
            {
                return NotFound();
            }

            return View(personInformation);
        }
    }

    // Post
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(string id)
    {
        using (var dbContext = new FbtContext())
        {
            var personInformation = dbContext.PersonInformations.Find(id);
            if (personInformation == null)
            {
                return NotFound();
            }

            // Xoá Data trong Table Account trước
            var account = dbContext.Accounts.Find(id);
            if (account != null)
            {
                dbContext.Accounts.Remove(account);
            }

            // Delete Data trong table tương ứng dựa và Id
            string idPrefix = id.Substring(0, 3);
            if (idPrefix == "HSS")
            {
                var student = dbContext.Students.Find(id);
                if (student != null)
                {
                    dbContext.Students.Remove(student);
                }
            }
            else if (idPrefix == "HSE")
            {
                var teacher = dbContext.Teachers.Find(id);
                if (teacher != null)
                {
                    dbContext.Teachers.Remove(teacher);
                }
            }
            else if (idPrefix == "HSP")
            {
                var parent = dbContext.Parents.FirstOrDefault(p => p.ParentId == id);
                if (parent != null)
                {
                    dbContext.Parents.Remove(parent);
                }
            }

            // Delete Data trong PersonInformation Table sau cùng
            dbContext.PersonInformations.Remove(personInformation);
            dbContext.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }
    }

//// ====================================== Manager Schedule ======================================

//  --------------------------------------- Schedule Class ---------------------------------------
    // Get
    public IActionResult ManagerTimeTable()
    {
        DateTime dateTime = DateTime.Now;

        using (var dbContext = new FbtContext())
        {

            var dayStart = dateTime.AddDays(-(int)dateTime.DayOfWeek + (int)DayOfWeek.Monday).Date;
            var dayEnd = dayStart.AddDays(6);

            
            var schoolYearRun = dbContext.SchoolYears.FirstOrDefault(item => item.DateStart <= dayStart && item.DateEnd >= dayEnd);
            if(schoolYearRun != null)
            {
                var gradesRun = dbContext.Grades.FirstOrDefault(item => item.SchoolYearId == schoolYearRun.SchoolYearId);
                if(gradesRun != null)
                {
                    var classesRun = dbContext.Classes.FirstOrDefault(item => item.GradeId == gradesRun.GradeId);
                    if(classesRun != null)
                    {
                        
                        ManagerTimeTable(schoolYearRun.SchoolYearId, gradesRun.GradeId, classesRun.ClassId, dayStart.ToString()+"&&"+dayEnd.ToString());
                    }
                }
            }
            //ViewBag.SchoolYears = dbContext.SchoolYears.ToList();
            //ViewBag.Grades = dbContext.Grades.ToList();
            //ViewBag.Class = dbContext.Classes.ToList();
            //ViewBag.WeekSchedules = dbContext.Schedules.Select(item => item.WeekBegins).Distinct().ToList();
            
            return View();
        }
    }

    // Post
    [HttpPost]
    public IActionResult ManagerTimeTable(string schoolYear, string gradeID, string classID, string weekBegin)
    {
        using (var dbContext = new FbtContext())
        {
            //ViewBag.SchoolYears = dbContext.SchoolYears.ToList();
            //ViewBag.Grades = dbContext.Grades.ToList();
            //ViewBag.Class = dbContext.Classes.ToList();
            //ViewBag.WeekSchedules = dbContext.Schedules.Select(item => item.WeekBegins).Distinct().ToList();

            var schoolYeardb = dbContext.SchoolYears.Any(s => s.SchoolYearId == schoolYear);

            if (schoolYeardb)
            {
                //Console.WriteLine("=====================================================");
                //Console.WriteLine("School Year: " + schoolYeardb);
                var gradeNamedb = dbContext.Grades.Any(s => s.GradeId == gradeID);
                if(gradeNamedb)
                {
                    //Console.WriteLine("Grade: " + gradeNamedb);
                    var classNamedb = dbContext.Classes.FirstOrDefault(s => s.ClassId == classID);
                    if(classNamedb != null)
                    {
                        //Console.WriteLine("Classes: " + classNamedb.ClassId);
                        string [] time = weekBegin.Split("&&");
                        DateTime weekBeginConvert;
                        DateTime weekEndConvert;
                        //Console.WriteLine(time[0] + " --- " + time[1]);

                        bool success1 = DateTime.TryParse(time[0], out weekBeginConvert);
                        bool success2 = DateTime.TryParse(time[1], out weekEndConvert);

                        if (success1 && success2)
                        {
                            Console.WriteLine(weekBeginConvert + "    " + weekEndConvert);
                            DateTime format = weekEndConvert.AddHours(24).AddSeconds(-1);
                            var scheduledb = dbContext.Schedules.Where(s => s.ClassId == classNamedb.ClassId && s.WeekBegins >= weekBeginConvert && s.WeekEnds <= weekEndConvert)
                                .Include(s => s.Class.Grade.SchoolYear)
                                .Include(s => s.Teacher.TeacherNavigation)
                                .Include(s => s.Subject).ToList();
                            //Console.WriteLine("Data: "+ scheduledb + " with " + schoolYear + " " + gradeID + " " + classID + " " + weekBeginConvert + " " + format);

                            var cookieOptions = new CookieOptions();
                            cookieOptions.Expires = DateTime.Now.AddDays(1);
                            Response.Cookies.Append("SchoolYearID", schoolYear);
                            Response.Cookies.Append("GradeID", gradeID);
                            Response.Cookies.Append("ClassID", classID);
                            Response.Cookies.Append("WeekBegins", weekBeginConvert.ToString());

                            return View(scheduledb);

                        }else if (!success1 || !success2)
                        {
                            Console.WriteLine("Time is not true format!");
                        }
                    }
                }
            }
        }
        Console.WriteLine("Schedule empty: " + schoolYear + gradeID + classID + weekBegin);
        return View();
    }

//  --------------------------------------- Schedule Teacher ---------------------------------------
    // Get
    public IActionResult ManagerTimeTableOfTeacher()
    {
        DateTime dateTime = DateTime.Now;

        using (var dbContext = new FbtContext())
        {
            var teacherID = dbContext.Teachers.Select(item => item.TeacherId).FirstOrDefault();
            var scheduledb = dbContext.Schedules.Where(item => item.TeacherId == teacherID && item.WeekBegins <= dateTime && item.WeekEnds >= dateTime)
                        .Include(s => s.Class.Grade.SchoolYear)
                        .Include(s => s.Teacher.TeacherNavigation)
                        .Include(s => s.Subject).ToList();
            return View(scheduledb);
        }
        return View();
    }

    // Post
    [HttpPost]
    public IActionResult ManagerTimeTableOfTeacher( string datetime, string teacherID)
    {
        using(var dbContext = new FbtContext())
        {
            string[] time = datetime.Split("&&");
            DateTime weekBeginConvert;
            DateTime weekEndConvert;

            bool success1 = DateTime.TryParse(time[0], out weekBeginConvert);
            bool success2 = DateTime.TryParse(time[1], out weekEndConvert);

            if (success1 && success2)
            {
                var scheduledb = dbContext.Schedules.Where(item => item.TeacherId == teacherID && item.WeekBegins >= weekBeginConvert && item.WeekEnds <= weekEndConvert)
                            .Include(s => s.Class.Grade.SchoolYear)
                            .Include(s => s.Teacher.TeacherNavigation)
                            .Include(s => s.Subject).ToList();
                Console.WriteLine("Teacher ID: " + teacherID + " Week Begins: " + weekBeginConvert + " Week Ends: " + weekEndConvert);
                return View(scheduledb);

            }
        }
        Console.WriteLine("Empty Teacher ID: " + teacherID + " Week Begins: " + datetime);
        return View();
    }

//  --------------------------------------- Create Schedule ---------------------------------------
    // Get
    public IActionResult CreateTimeTable()
    {
        using(var dbContext = new FbtContext())
        {
            ViewBag.SchoolYears = dbContext.SchoolYears.ToList();
            ViewBag.Grades = dbContext.Grades.ToList();
            ViewBag.Class = dbContext.Classes.ToList();
            ViewBag.Subject = dbContext.Subjects.ToList();
            ViewBag.TeachingSubject = dbContext.SubjectTeachers.ToList();
            ViewBag.PersonInformation = dbContext.PersonInformations.ToList();
        }
        return View();
    }

    // Post
    [HttpPost]
    public IActionResult CreateTimeTable(string obj)
    {
        List<Schedule> schedules = new List<Schedule>();
        schedules = Handle.SplitScheduleInput(obj);
        schedules.ForEach(schedule =>
        {
            using(var dbContext = new FbtContext())
            {
                var scheduleCheck = dbContext.Schedules.Any(item => item.ClassId == schedule.ClassId && item.DayOfWeek == schedule.DayOfWeek && item.TimeStart == schedule.TimeStart && item.TimeEnd == schedule.TimeEnd);
                if (!scheduleCheck)
                {
                    var scheduleCheck2 = dbContext.Schedules.Any(item => item.TeacherId == schedule.TeacherId && item.DayOfWeek == schedule.DayOfWeek && item.TimeStart == schedule.TimeStart && item.TimeEnd == schedule.TimeEnd);
                    if (!scheduleCheck2)
                    {
                        schedule.ScheduleId = Handle.CreateScheduleID();
                        dbContext.Schedules.Add(schedule);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("FailCreate", "Giờ học: " + schedule.TimeStart + " - " + schedule.TimeEnd + " đã được sắp xếp cho giáo viên. Vui lòng kiểm tra lại trước khi nhập vào!");
                    }
                }
                else
                {
                    ModelState.AddModelError("FailCreate", "Giờ học: " + schedule.TimeStart + " - " + schedule.TimeEnd + " đã có. Vui lòng kiểm tra lại trước khi nhập vào!");
                }
            }
        });
        return View();
    }

//  --------------------------------------- Update Schedule ---------------------------------------
    [HttpPost]
    public IActionResult UpdateTimeTable(string scheduleEdit)
    {
        string[] element = scheduleEdit.Split('$');
        if(element.Length > 0)
        {
            using(var dbContext = new FbtContext())
            {
                var schedule = dbContext.Schedules.FirstOrDefault(item => item.ScheduleId == element[0]);
                if (schedule != null)
                {
                    //var schedule = scheduleID + "$" + teacherID.value + "$" + subjectID.value + "$" + timeStart.value + "$" + timeEnd.value;
                    var checkSubject = dbContext.SubjectTeachers.Any(item => item.SubjectId == element[2] && item.TeacherId == element[1] && item.ClassId == schedule.ClassId);
                    if (checkSubject)
                    {
                        TimeSpan updateTimeStart;
                        TimeSpan updateTimeEnd;

                        if(TimeSpan.TryParse(element[3], out updateTimeStart) && TimeSpan.TryParse(element[4], out updateTimeEnd) && updateTimeStart <= updateTimeEnd)
                        {
                            schedule.SubjectId = element[2];
                            schedule.TeacherId = element[1];
                            schedule.TimeStart = updateTimeStart;
                            schedule.TimeEnd = updateTimeEnd;
                            dbContext.Update(schedule);
                            dbContext.SaveChanges();
                        }
                        
                    }
                }
            }

        }
        return View();
    }

//  --------------------------------------- Delete Schedule ---------------------------------------
    [HttpPost]
    public IActionResult DeleteTimeTable(string scheduleDelete)
    {
        using(var dbContext = new FbtContext())
        {
            var schedule = dbContext.Schedules.FirstOrDefault(item => item.ScheduleId == scheduleDelete);
            if(schedule != null)
            {
                dbContext.Remove(schedule);
                dbContext.SaveChanges();
            }
        }
        return View();
    }

//// ====================================== Get Data Javascript ======================================
    public IActionResult GetSchoolYears()
    {
        using(var dbContext = new FbtContext())
        {
            List<SchoolYear> schedules = dbContext.SchoolYears.ToList();
            return Json(schedules);
        }
    }

    public IActionResult GetGrades()
    {
        using (var dbContext = new FbtContext())
        {
            return Json(dbContext.Grades.ToList());
        }
    }

    public IActionResult GetClasses()
    {
        using (var dbContext = new FbtContext())
        {
            return Json(dbContext.Classes.ToList());
        }
    }

    public IActionResult GetSubjects()
    {
        using (var dbContext = new FbtContext())
        {
            return Json(dbContext.Subjects.ToList());
        }
    }

    public IActionResult GetSubjectTeacher()
    {
        using (var dbContext = new FbtContext())
        {
            return Json(dbContext.SubjectTeachers.ToList());
        }
    }

    public IActionResult GetPersonInformation()
    {
        using (var dbContext = new FbtContext())
        {
            return Json(dbContext.PersonInformations.Where(item => item.Id.StartsWith("HSE")).ToList());
        }
    }

}
