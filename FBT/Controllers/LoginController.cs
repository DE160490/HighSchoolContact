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

    }
}
