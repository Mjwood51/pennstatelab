using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PennState
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "DeletePhoto",
                "item/deletephoto/{photoId}/{itemId}/{subId}/{name}",
                new { controller = "Item", action = "DeletePhoto" });

            routes.MapRoute(
                "DeleteFile",
                "item/deletefile/{id}/{itemId}",
                new { controller = "Item", action = "DeleteFile" });

            routes.MapRoute(
                "DeleteItem",
                "item/deleteitem/{id}",
                new { controller = "Item", action = "DeleteItem" });

            routes.MapRoute(
                "Edit",
                "item/edit/{cid}",
                new { controller = "Item", action = "Edit" });

            routes.MapRoute(
                "GetItemList",
                "item/getitemlist/{location}/{type}/{vendor}/{owner}",
                new { controller = "Item", action = "GetItemList" });

            routes.MapRoute(
                "Registration",
                "account/registration/{id}",
                new { controller = "Account", action = "Registration" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
