using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FBT.Filter
{
    public class LoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.Controller.GetType().Name;
            var Username = context.HttpContext.Session.GetString("Username");
            if(Username == null && controllerName != "LoginController")
            {
                context.Result = new RedirectResult("/Login");
            }else if(Username != null && controllerName != "LoginController")
            {
                var role = Username.Split('$')[2];
                if((role == "0" &&  controllerName != "StudentController") 
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
