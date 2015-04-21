﻿using System.Web.Mvc;
using System.Web.Routing;

namespace EPM.Extension.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default_MeteringPoint",
                url: "MeteringPoint/{id}",
                defaults: new { controller = "MeteringCode", action = "Detail" }
            );
            routes.MapRoute(
                name: "Default_MeteringPointStandardInfo",
                url: "StandortInfoV1/{codeOrId}",
                defaults: new { controller = "MeteringCode", action = "StandardInfo" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Customer", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
