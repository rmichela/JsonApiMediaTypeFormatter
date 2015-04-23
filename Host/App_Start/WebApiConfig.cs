using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using JsonApi;
using JsonApi.Serialization;

namespace Host
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Add(new JsonApiMediaTypeFormatter());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
