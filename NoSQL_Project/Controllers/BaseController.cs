using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Models;

namespace NoSQL_Project.Controllers
{
    public class BaseController : Controller
    {
        protected Employee CurrentUser => HttpContext.Session.GetObject<Employee>("LoggedInUser");

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var isLoginPage = context.RouteData.Values["controller"]?.ToString() == "Employees"
                           && (context.RouteData.Values["action"]?.ToString() == "Login" ||
                               context.RouteData.Values["action"]?.ToString() == "Logout");

            if (!isLoginPage && CurrentUser == null)
            {
                context.Result = RedirectToAction("Login", "Employees");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
