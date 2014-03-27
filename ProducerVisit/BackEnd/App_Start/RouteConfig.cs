using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BackEnd
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            // ToDo: add other "actions" (reports) here.
            routes.MapRoute(
                name: "SummaryReport",
                url: "Report/Summary/{id}",
                defaults: new { controller = "Visit", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Visit", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}