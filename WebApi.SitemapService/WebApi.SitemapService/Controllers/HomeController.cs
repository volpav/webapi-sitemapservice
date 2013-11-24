using System.Web.Mvc;

namespace WebApi.SitemapService.Controllers
{
    /// <summary>
    /// Represents home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Returns the default view.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
