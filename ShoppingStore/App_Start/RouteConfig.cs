using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShoppingStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Shop", "Shop/{action}/{name}", new { controller = "shop", action = "Index",name =  UrlParameter.Optional }, new[] { "ShoppingStore.Controllers" });

            routes.MapRoute("SidebarPartial", "page/SlidebarPartial", new { controller = "Pages", action = "SidebarPartial" }, new[] { "ShoppingStore.Controllers" });
            routes.MapRoute("PagesMenuPartial", "page/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" }, new[] {"ShoppingStore.Controllers"});
            routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" }, new[] { "ShoppingStore.Controllers" });
            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" }, new[] { "ShoppingStore.Controllers" });

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
