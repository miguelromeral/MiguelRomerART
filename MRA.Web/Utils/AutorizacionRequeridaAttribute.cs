using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MRA.Web.Utils
{

    public class AutorizacionRequeridaAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userid = context.HttpContext.Session.GetString(SessionSettings.USER_ID) ?? "";

            if (!context.HttpContext.User.Identity.IsAuthenticated || String.IsNullOrEmpty(userid))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Admin",
                    action = "Login"
                }));
            }
        }
    }
}
