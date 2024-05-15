using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApiMafperu
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API 
            // EnableCors
            //config.EnableCors();
            //config.EnableCors(new EnableCorsAttribute("*", "*", "POST"));
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Rutas de API web
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
