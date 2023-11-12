using Microsoft.AspNetCore.Mvc;

namespace FBT.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
