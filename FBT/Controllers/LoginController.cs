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
                    if(checkAccount.Role == 1)
                    {
                        return RedirectToAction("Index", "Student", new { studentId = account.AccountId});
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
