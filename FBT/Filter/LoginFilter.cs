using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FBT.Filter
{
    public class LoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("Username") == null)
            {
                Console.WriteLine("HelloFilter");
                //context.Result = new RedirectResult("/Login");
                context.Result = new RedirectToRouteResult("Login");
            }
        }
    }
}
