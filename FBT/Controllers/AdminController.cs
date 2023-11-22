using FBT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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



    //--------------------------------------------DETAIlS
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

}
