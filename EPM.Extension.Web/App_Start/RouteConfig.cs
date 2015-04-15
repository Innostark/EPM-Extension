using System.Web.Mvc;
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
                name: "Default_MeteringPointEditNew",
                url: "StandortInfoV1/{id}",
                defaults: new { controller = "MeteringCode", action = "EditNew" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Customer", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
