using FBT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FBT.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Account account) {

            using (var dbContext = new FbtContext())
            {
                Account? getAccount = dbContext.Accounts.FirstOrDefault(s => s.AccountId == account.AccountId && s.Password == account.Password);
                if (getAccount != null)
                {
                    var cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("Username", getAccount.AccountId + "$" + getAccount.Password + "$" + getAccount.Role, cookieOptions);
                    HttpContext.Session.SetString("Username", getAccount.AccountId + "$" + getAccount.Password + "$" + getAccount.Role);
                    if(getAccount.Role == 0)
                    {
                        return RedirectToAction("Index", "Student", new { studentId = account.AccountId });
                    }else if(getAccount.Role == 1)
                    {
                        return RedirectToAction("Index", "Teacher", new { teacherId = account.AccountId });
                    }else if(getAccount.Role == 2)
                    {
                        return RedirectToAction("Index", "Parent", new { parentId = account.AccountId });
                    }else if(getAccount.Role == 3)
                    {
                        return RedirectToAction("Index", "Admin", new { adminId = account.AccountId });
                    }
                    else
                    {
                        return NotFound();
                    }  
                }
                else
                {
                    return View(account);
                }
            }
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgetPassword(string AccountId)
        {
            using(var dbContext = new FbtContext())
            {
                var personInformation = dbContext.PersonInformations.Include(a => a.Account).FirstOrDefault(a => a.Id == AccountId);
                if (personInformation == null)
                {
                    ModelState.AddModelError("AccountId", "ID không tồn tại!");
                }

                Random random = new Random();
                string randomString = new string(Enumerable.Repeat(0, 6).Select(i => (char)(random.Next(10) + '0')).ToArray());
                HttpContext.Session.SetString("UserResetPassword", randomString + "$" + AccountId);
                Console.WriteLine(randomString);
            }
            return View();
        }

        [HttpPost]
        public IActionResult ForgetPasswordConfirm(string code)
        {
            var codeSession = HttpContext.Session.GetString("UserResetPassword");

            if(code != codeSession.Split("$")[0])
            {
                return RedirectToAction("ForgetPassword");
            }

            return RedirectToAction("ResetPassword");
        }

        public IActionResult ResetPassword() { 
            return View(); 
        }

        [HttpPost]
        public IActionResult ResetPassword(string password, string confirmpassword)
        {
            if(password == confirmpassword)
            {
                var accountId = HttpContext.Session.GetString("UserResetPassword").Split("$")[1];

                using (var dbContext = new FbtContext())
                {
                    var account = dbContext.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                    account.Password = password;
                    dbContext.SaveChanges();
                }
                return RedirectToAction("Login");
            }
            return RedirectToAction("ResetPassword");
        }
    }
}
