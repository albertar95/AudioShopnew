using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AudioShopFrontend
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Category",
                url: "{action}/{Nid}/{Title}",
                defaults: new { controller = "Home",action = "", Nid = "", Title = UrlParameter.Optional }
            );
            //routes.MapRoute(
            //    name: "Product",
            //    url: "{action}/{ProductNumber}/{Title}",
            //    defaults: new { controller = "Home", action = "Product", ProductNumber = "", Title = UrlParameter.Optional }
            //);
        }
    }
}
