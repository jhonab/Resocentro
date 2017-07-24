using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Encuesta
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Tecnologo",
                url: "Tecnologo/{action}/{id}",
                defaults: new
                {
                    controller = "RealizarExamen",
                    action = "ListaEspera",
                    id = UrlParameter.Optional
                }
            );
            routes.MapRoute(
               name: "Supervisor",
               url: "Supervisor/{action}/{id}",
               defaults: new
               {
                   controller = "SupervisarExamen",
                   action = "ListaEspera",
                   id = UrlParameter.Optional
               }
           );
            routes.MapRoute(
               name: "Encuestador",
               url: "Encuestador/{action}/{id}",
               defaults: new
               {
                   controller = "RealizarEncuesta",
                   action = "ListaEspera",
                   id = UrlParameter.Optional
               }
           );
            routes.MapRoute(
                          name: "VeriricarAtencion",
                          url: "Atencioncliente/{action}/{id}",
                          defaults: new
                          {
                              controller = "Atencion",
                              action = "ListaEspera",
                              id = UrlParameter.Optional
                          }
                      );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}