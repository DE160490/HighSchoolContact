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
            List<Account> objAccountList = dbContext.Accounts.Include(a => a.AccountNavigation).ToList();
            return View(objAccountList);
        }
    }


    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Fullname,Gender,Dob,PlaceOfBirth,PlaceOfResidence,Phone,Email,Ethnic,Religion")] PersonInformation personInformation, string Password)
    {
        using (var dbContext = new FbtContext())
        {
            ModelState.Remove("Account");
            if (ModelState.IsValid)
            {
                // Add the PersonInformation to the context and save changes
                dbContext.PersonInformations.Add(personInformation);
                dbContext.SaveChanges();

                if (personInformation.Account == null)
                {
                    personInformation.Account = new Account();
                }


                // Create the Account
                var account = personInformation.Account;
                account.AccountId = personInformation.Id;  // Assign PersonInformation.Id to Account.AccountId
                //account.Password = "password123";
                account.Password = Password;
                //account.Password = personInformation.Account.Password;

                // Determine the role based on the ID prefix
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
                    account.Role = -1; // Default value
                }

                // Add the Account to the context and save changes
                dbContext.Accounts.Add(account);
                dbContext.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }

            // If the model state is not valid, return the view with the current PersonInformation
            return View(personInformation);
        }
    }


}
