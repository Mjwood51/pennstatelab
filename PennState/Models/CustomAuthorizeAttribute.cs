using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PennState.Models;

namespace PennState.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private PennStateDB _context;
        public CustomAuthorizeAttribute()
        {
            _context = new PennStateDB();
        }
        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);
            if(!isAuthorized)
            {
                return false;
            }
            if (Roles.Split(',').Contains(_context.Tbl_Users.Where(x => x.UserName == httpContext.User.Identity.Name).FirstOrDefault().Tbl_Roles.RoleName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RedirectToRouteResult routeData = null;

            if (CurrentUser == null)
            {
                routeData = new RedirectToRouteResult
                    (new System.Web.Routing.RouteValueDictionary
                    (new
                    {
                        controller = "Account",
                        action = "Login",
                    }
                    ));
            }
            else
            {
                routeData = new RedirectToRouteResult
                (new System.Web.Routing.RouteValueDictionary
                 (new
                 {
                     controller = "Error",
                     action = "AccessDenied"
                 }
                 ));
            }

            filterContext.Result = routeData;
        }
    }
}