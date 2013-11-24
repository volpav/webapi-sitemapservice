using System.Collections.Concurrent;
using System.ServiceModel;
using WebApi.SitemapService.BackgroundWorker;

namespace WebApi.SitemapService.Models
{
    /// <summary>
    /// Represents a sitemap store. This class cannot be inherited.
    /// </summary>
    public sealed class SitemapManager
    {
        /// <summary>
        /// Represents a sitemap service callback. This class cannot be inherited.
        /// </summary>
        private sealed class SitemapServiceCallback : ISitemapServiceCallback
        {
            /// <summary>
            /// Occurs every time the process of sitemap parsing progresses.
            /// </summary>
            /// <param name="url">Website URL.</param>
            /// <param name="percentage">Progress percentage.</param>
            public void OnProgress(string url, int percentage)
            {
                SitemapManager.Current.OnProgress(url, percentage);
            }

            /// <summary>
            /// Occurs when the process of sitemap parsing has finished.
            /// </summary>
            /// <param name="url">Website URL.</param>
            /// <param name="sitemap">Parsed sitemap.</param>
            public void OnCompleted(string url, SitemapNode sitemap)
            {
                SitemapManager.Current.OnCompleted(url, sitemap);
            }
        }

        /// <summary>
        /// Represents sitemap service operation. This class cannot be inherited.
        /// </summary>
        private sealed class SitemapServiceOperation
        {
            /// <summary>
            /// Gets or sets the website URL.
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// Gets or sets the percentage.
            /// </summary>
            public int Percentage { get; set; }

            /// <summary>
            /// Gets or sets the result.
            /// </summary>
            public SitemapNode Result { get; set; }
        }

        private ConcurrentDictionary<string, SitemapServiceOperation> _operations;
        private static SitemapManager _current = new SitemapManager();

        /// <summary>
        /// Gets the current store.
        /// </summary>
        public static SitemapManager Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        private SitemapManager()
        {
            _operations = new ConcurrentDictionary<string, SitemapServiceOperation>();
        }

        /// <summary>
        /// Begins parsing sitemap.
        /// </summary>
        /// <param name="url">Website URL.</param>
        public void BeginParseSitemap(string url)
        {
            if (!_operations.ContainsKey(url))
                new SitemapServiceClient(new InstanceContext(new SitemapServiceCallback())).BeginParseSitemap(url);
        }

        /// <summary>
        /// Returns the progress of parsing the given URL.
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <returns>Progress (percentage).</returns>
        public int GetProgress(string url)
        {
            int ret = 0;
            SitemapServiceOperation operation = null;

            if (_operations.TryGetValue(url, out operation))
                ret = operation.Percentage;

            return ret;
        }

        /// <summary>
        /// Returns sitemap for a given URL (if available).
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <returns>Sitemap.</returns>
        public SitemapNode GetResult(string url)
        {
            SitemapNode ret = null;
            SitemapServiceOperation operation = null;

            if (_operations.TryGetValue(url, out operation))
                ret = operation.Result;

            return ret;
        }

        /// <summary>
        /// Occurs every time the process of sitemap parsing progresses.
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <param name="percentage">Progress percentage.</param>
        private SitemapServiceOperation OnProgress(string url, int percentage)
        {
            return _operations.AddOrUpdate(url, new SitemapServiceOperation() { 
                Url = url, 
                Percentage = percentage 
            }, (k, e) => { 
                e.Percentage = percentage; 
                return e; 
            });
        }

        /// <summary>
        /// Occurs when the process of sitemap parsing has finished.
        /// </summary>
        /// <param name="url">Website URL.</param>
        /// <param name="sitemap">Parsed sitemap.</param>
        private void OnCompleted(string url, SitemapNode sitemap)
        {
            OnProgress(url, 100).Result = sitemap;
        }
    }
}