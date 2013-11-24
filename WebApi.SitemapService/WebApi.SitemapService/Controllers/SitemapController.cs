using System.Web.Http;
using WebApi.SitemapService.BackgroundWorker;

namespace WebApi.SitemapService.Controllers
{
    /// <summary>
    /// Represents sitemap controller.
    /// </summary>
    public class SitemapController : ApiController
    {
        /// <summary>
        /// Begins parsing sitemap.
        /// </summary>
        /// <param name="url">Website URL.</param>
        [HttpGet]
        public void BeginParseSitemap(string url)
        {
            Models.SitemapManager.Current.BeginParseSitemap(url);
        }

        /// <summary>
        /// Returns the progress of parsing the given URL.
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <returns>Progress (percentage).</returns>
        [HttpGet]
        public int GetProgress(string url)
        {
            return Models.SitemapManager.Current.GetProgress(url);
        }

        /// <summary>
        /// Returns sitemap for a given URL (if available).
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <returns>Sitemap.</returns>
        [HttpGet]
        public SitemapNode GetResult(string url)
        {
            return Models.SitemapManager.Current.GetResult(url);
        }
    }
}