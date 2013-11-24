using System.Web.Http;

namespace WebApi.SitemapService
{
    /// <summary>
    /// Provides methods for configuring Web API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers Web API components.
        /// </summary>
        /// <param name="config">Configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );
        }
    }
}
