﻿using FBT.Models;
using FBT.Reponsitory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Principal;
using X.PagedList;

namespace FBT.Controllers;

public class AdminController : Controller
{
    // GET: AdminController
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

    //---------------------------------------CREATE

    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Fullname,Gender,Dob,PlaceOfBirth,PlaceOfResidence,Phone,Email,Ethnic,Religion")] PersonInformation personInformation, string Password, string Role)
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
            //if (dbContext.PersonInformations.Any(p => p.Id == personInformation.Id))
            //{
            //    ModelState.AddModelError("Id", "A person with this Id already exists.");
            //}
            if (ModelState.IsValid)
            {

                dbContext.PersonInformations.Add(personInformation);
                dbContext.SaveChanges();

                if (personInformation.Account == null)
                {
                    personInformation.Account = new Account();
                }


                // Create the Account
                var account = personInformation.Account;
                account.AccountId = personInformation.Id;
                //account.Password = "password123";
                account.Password = Password;


                string idPrefix = personInformation.Id.Substring(0, 3);
                if (idPrefix == "HSS")
                {
                    account.Role = 0;
                }
                else if (idPrefix == "HSP")
                {
                    account.Role = 1;
                }
                else if (idPrefix == "HSE")
                {
                    account.Role = 2;
                }
                else
                {
                    account.Role = -1;
                }

                dbContext.Accounts.Add(account);
                dbContext.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }

            return View(personInformation);
        }
    }

    //--------------------------------------------------EDIT
    // GET: Admin/Edit/5
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

    //// Thời khóa biểu Admin
    ///
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
                Console.WriteLine("=====================================================");
                Console.WriteLine("School Year: " + schoolYeardb);
                var gradeNamedb = dbContext.Grades.Any(s => s.GradeId == gradeID);
                if(gradeNamedb)
                {
                    Console.WriteLine("Grade: " + gradeNamedb);
                    var classNamedb = dbContext.Classes.FirstOrDefault(s => s.ClassId == classID);
                    if(classNamedb != null)
                    {
                        Console.WriteLine("Classes: " + classNamedb.ClassId);
                        string [] time = weekBegin.Split("&&");
                        DateTime weekBeginConvert;
                        DateTime weekEndConvert;
                        Console.WriteLine(time[0] + " --- " + time[1]);

                        bool success1 = DateTime.TryParse(time[0], out weekBeginConvert);
                        bool success2 = DateTime.TryParse(time[1], out weekEndConvert);

                        if (success1 && success2)
                        {
                            Console.WriteLine(weekBeginConvert + "    " + weekEndConvert);
                            DateTime format = weekEndConvert.AddHours(24).AddSeconds(-1);
                            var scheduledb = dbContext.Schedules.Where(s => s.ClassId == classNamedb.ClassId && s.WeekBegins <= weekBeginConvert && s.WeekEnds >= weekEndConvert)
                                .Include(s => s.Class.Grade.SchoolYear)
                                .Include(s => s.Teacher.TeacherNavigation)
                                .Include(s => s.Subject).ToList();
                            Console.WriteLine("Data: "+ scheduledb + " with " + schoolYear + " " + gradeID + " " + classID + " " + weekBeginConvert + " " + format);

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

    [HttpPost]
    public IActionResult ManagerTimeTableOfTeacher( string datetime, string teacherID)
    {
        using(var dbContext = new FbtContext())
        {
            string[] time = datetime.Split("&&");
            DateTime weekBeginConvert;
            DateTime weekEndConvert;
            Console.WriteLine(time[0] + " --- " + time[1]);

            bool success1 = DateTime.TryParse(time[0], out weekBeginConvert);
            bool success2 = DateTime.TryParse(time[1], out weekEndConvert);

            if (success1 && success2)
            {
                var scheduledb = dbContext.Schedules.Where(item => item.TeacherId == teacherID && item.WeekBegins <= weekBeginConvert && item.WeekEnds >= weekEndConvert)
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

    [HttpPost]
    public IActionResult CreateTimeTable(string obj)
    {
        List<Schedule> schedules = new List<Schedule>();
        schedules = Handle.SplitScheduleInput(obj);
        schedules.ForEach(schedule =>
        {
            var getSchedule = schedule.ScheduleId + " " + schedule.TeacherId + " " + schedule.ClassId + " " + schedule.SubjectId + " " +
            schedule.WeekBegins + " " + schedule.WeekEnds + " " + schedule.DayOfWeek + " " + schedule.Lecture + " " + schedule.TimeStart + " " + schedule.TimeEnd;
            Console.WriteLine(getSchedule);
        });
        return View();
    }

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


    //--------------------------------------------DETAIlS
    public IActionResult Details(string id)
    {
        using (var dbContext = new FbtContext())
        {
            // Retrieve the PersonInformation with the associated Account
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

    //--------------------------------------Delete
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

            // Delete the associated Account first
            var accountId = personInformation.Id;
            var account = dbContext.Accounts.Find(accountId);
            if (account != null)
            {
                dbContext.Accounts.Remove(account);
                dbContext.SaveChanges();
            }

            // Then, delete the PersonInformation
            dbContext.PersonInformations.Remove(personInformation);
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }


}
