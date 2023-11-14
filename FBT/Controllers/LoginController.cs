using FBT.Models;
using Microsoft.AspNetCore.Mvc;

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
            return View(account);
        }

    }
}
