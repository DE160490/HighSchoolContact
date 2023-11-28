using FBT.Filter;
using FBT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Text;

namespace FBT.Controllers
{
    public class LoginController : Controller
    {
//// ====================================== Login ======================================
        // Get
        [AllowAnonymous]
        public IActionResult Login()
        {
            var userID = HttpContext.Session.GetString("Username");
            if (userID != null)
            {
                Console.WriteLine(userID);
                Console.WriteLine("Hello");
                using (var dbContext = new FbtContext())
                {
                    var accountID = userID.Split('$')[0].Trim();
                    var password = userID.Split('$')[1].Trim();
                    var role = Convert.ToInt32(userID.Split('$')[2].Trim());
                    Account? getAccount = dbContext.Accounts.FirstOrDefault(s => s.AccountId == accountID && s.Password == password && s.Role == role);
                    if (getAccount != null)
                    {
                        return Login(getAccount);
                    }
                }
            }
            return View();
        }

        // Post
        [HttpPost]
        public IActionResult Login(Account account) {
            try
            {
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
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View();
            }
        }

//// ====================================== Forgetpassword ======================================
        // Get
        public IActionResult ForgetPassword()
        {
            return View();
        }

        // Post
        [HttpPost]
        public IActionResult ForgetPassword(string AccountId)
        {
            try {
                using (var dbContext = new FbtContext())
                {
                    var personInformation = dbContext.PersonInformations.Include(a => a.Account).FirstOrDefault(a => a.Id == AccountId);
                    if (personInformation == null)
                    {
                        ModelState.AddModelError("AccountId", "ID không tồn tại!");
                    }

                    Random random = new Random();
                    string randomString = new string(Enumerable.Repeat(0, 6).Select(i => (char)(random.Next(10) + '0')).ToArray());
                    HttpContext.Session.SetString("UserResetPassword", randomString + "$" + AccountId);
                    var message = MailUtils.SendGmail("thanhncde160490@fpt.edu.vn", "nguyencongthanh15082001@gmail.com", "Code Confirm Forgetpassword", randomString, "thanhncde160490@fpt.edu.vn", "password or keypassword");
                    Console.WriteLine(message);
                    Console.WriteLine(randomString);
                }
                return View();
            }
            catch(Exception ex) { 
                Console.WriteLine(ex.ToString());
                return View();
            }
        }
        
        // Post Code Confirm Password
        [HttpPost]
        public IActionResult ForgetPasswordConfirm(string code)
        {
            var codeSession = HttpContext.Session.GetString("UserResetPassword");
            if(codeSession != null)
            {
                if(code != codeSession.Split("$")[0])
                {
                    return RedirectToAction("ForgetPassword");
                }
            }
            return RedirectToAction("ResetPassword");
        }

        // Get in Reset Password
        public IActionResult ResetPassword() { 
            return View(); 
        }

        // Post in Reset Password
        [HttpPost]
        public IActionResult ResetPassword(string password, string confirmpassword)
        {
            if(password == confirmpassword)
            {
                var accountId = HttpContext.Session.GetString("UserResetPassword")?.Split("$")[1];
                if(accountId != null)
                {
                    using (var dbContext = new FbtContext())
                    {
                        var account = dbContext.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                        if (account != null)
                        {
                            account.Password = password;
                            dbContext.SaveChanges();
                        }
                    }
                    return RedirectToAction("Login");
                }
            }
            return RedirectToAction("ResetPassword");
        }
    }
}
