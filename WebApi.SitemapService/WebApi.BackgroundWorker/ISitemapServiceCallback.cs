using System.ServiceModel;

namespace WebApi.BackgroundWorker
{
    /// <summary>
    /// Provides a set of callbacks for <see cref="WebApi.BackgroundWorker.ISitemapService" />.
    /// </summary>
    public interface ISitemapServiceCallback
    {
        /// <summary>
        /// Occurs every time the process of sitemap parsing progresses.
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <param name="percentage">Progress percentage.</param>
        [OperationContract(IsOneWay = true)]
        void OnProgress(string url, int percentage);

        /// <summary>
        /// Occurs when the process of sitemap parsing has finished.
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <param name="sitemap">Parsed sitemap.</param>
        [OperationContract(IsOneWay = true)]
        void OnCompleted(string url, SitemapNode sitemap);
    }
}
