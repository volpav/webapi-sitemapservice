using System.Web.Optimization;

namespace WebApi.SitemapService
{
    /// <summary>
    /// Provides methods for configuring bundles.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Registers bundles.
        /// </summary>
        /// <param name="bundles">Bundle collection.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js", "~/Scripts/Sitemap.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
        }
    }
}