using Azure;
using FBT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace FBT.Filter
{
    public class LoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.Controller.GetType().Name;

            //var controllerNameNow = context.RouteData.Values["controller"];
            //var actionNameNow = context.RouteData.Values["action"];
            //Console.WriteLine(controllerNameNow + " " +actionNameNow);

            var Username = context.HttpContext.Session.GetString("Username");
            if(Username == null && controllerName != "LoginController")
            {
                context.Result = new RedirectResult("/Login");
            }else if(Username != null && controllerName != "LoginController")
            {
                //Console.WriteLine(controllerName);
                if (controllerName != "LogoutController")
                {
                    var role = Username.Split('$')[2];
                    if ((role == "0" && controllerName != "StudentController")
                        || (role == "1" && controllerName != "ParentController")
                        || (role == "2" && controllerName != "TeacherController")
                        || (role == "3" && controllerName != "AdminController"))
                    {
                        context.Result = new RedirectResult("/Login");
                    }
                }
            }
        }
    }
}
