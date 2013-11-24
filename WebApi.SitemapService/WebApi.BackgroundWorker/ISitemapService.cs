using System.ServiceModel;

namespace WebApi.BackgroundWorker
{
    /// <summary>
    /// Allows parsing website sitemap.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ISitemapServiceCallback))]
    public interface ISitemapService
    {
        /// <summary>
        /// Begins parsing sitemap.
        /// </summary>
        /// <param name="url">Website URL.</param>
        [OperationContract(IsOneWay = true)]
        void BeginParseSitemap(string url);
    }
}
