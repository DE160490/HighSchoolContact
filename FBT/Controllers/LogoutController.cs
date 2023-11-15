using FBT.Models;
using Microsoft.AspNetCore.Mvc;

namespace FBT.Controllers
{
    public class LogoutController : Controller
    {
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Username");
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
