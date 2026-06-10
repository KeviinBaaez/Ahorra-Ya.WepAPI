using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AhorraYa.WebClient.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuted(context);
        }
    }
}
