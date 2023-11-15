using FBT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBT.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Account account) {
            using (var dbContext = new FbtContext())
            {
                Console.WriteLine(account + " " + account.AccountId + " " + account.Password);
                Account? checkAccount = dbContext.Accounts.FirstOrDefault(s => s.AccountId == account.AccountId);
                if (checkAccount != null)
                {
                    HttpContext.Session.SetString("username", account.AccountId);
                    if(checkAccount.Role == 0)
                    {
                        return RedirectToAction("Index", "Student", new { studentId = account.AccountId});
                    }else if(checkAccount.Role == 1)
                    {
                        return RedirectToAction("Index", "Teacher", new { teacherId = account.AccountId });
                    }else if(checkAccount.Role == 2)
                    {
                        return RedirectToAction("Index", "Parent", new { parentId = account.AccountId });
                    }else if(checkAccount.Role == 3)
                    {
                        return RedirectToAction("Index", "Admin", new { adminId = account.AccountId });
                    }
                    else
                    {
                        return NotFound();
                    }
                    return View(account);
                }
                else
                {
                    return View(account);
                }
            }
        }

    }
}
