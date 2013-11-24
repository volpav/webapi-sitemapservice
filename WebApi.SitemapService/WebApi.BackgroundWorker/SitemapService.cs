using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace WebApi.BackgroundWorker
{
    /// <summary>
    /// Represents a sitemap parser service.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SitemapService : ISitemapService
    {
        /// <summary>
        /// Gets the client callback.
        /// </summary>
        private ISitemapServiceCallback Callback
        {
            get { return OperationContext.Current.GetCallbackChannel<ISitemapServiceCallback>(); }
        }

        /// <summary>
        /// Gets the maximum number of links to parse.
        /// </summary>
        private int MaxLinks
        {
            get { return 50; }
        }

        /// <summary>
        /// Gets or sets the maximum traverse depth.
        /// </summary>
        private int MaxDepth
        {
            get { return 4; }
        }

        /// <summary>
        /// Gets the maximum number of links to be parsed on any level.
        /// </summary>
        private int MaxLevelSize
        {
            get { return 10; }
        }

        /// <summary>
        /// Gets the current number of parsed links.
        /// </summary>
        private int CurrentLinks { get; set; }

        /// <summary>
        /// Gets or sets the website URL.
        /// </summary>
        private string Url { get; set; }

        /// <summary>
        /// Gets or sets the original URL.
        /// </summary>
        private string OriginalUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the list of already parsed links.
        /// </summary>
        private IDictionary<string, string> ParsedLinks { get; set; }

        /// <summary>
        /// Gets or sets the mapping between link URLs and sitemap nodes.
        /// </summary>
        private IDictionary<string, SitemapNode> NodesToLinks { get; set; }

        /// <summary>
        /// Begins parsing sitemap.
        /// </summary>
        /// <param name="url">Website URL.</param>
        public void BeginParseSitemap(string url)
        {
            SitemapNode root = null;

            CurrentLinks = 0;
            OriginalUrl = url;
            Url = AddProtocol(url);
            ParsedLinks = new Dictionary<string, string>();
            NodesToLinks = new Dictionary<string, SitemapNode>();

            root = ToNode(Url);

            ParseRecursive(Url, root.Children, 1);
            FillTitles();

            Callback.OnCompleted(OriginalUrl, root);

            // Firing 100% completion after we've got the results.
            // Prevents the client from reporting "no data" before it gets it.
            Callback.OnProgress(OriginalUrl, 100);
        }

        /// <summary>
        /// Recursively parses the given URL and all links within it.
        /// </summary>
        /// <param name="url">URL to parse.</param>
        /// <param name="addTo">A collection of nodes to add results to.</param>
        private void ParseRecursive(string url, IList<SitemapNode> addTo, int depth)
        {
            int addedLinks = 0;
            SitemapNode node = null;
            string absoluteUrl = string.Empty;
            string html = DownloadString(url);
            List<SitemapNode> addedNodes = new List<SitemapNode>();

            if (!string.IsNullOrEmpty(html))
            {
                // Associating HTML with self
                if (!ParsedLinks.ContainsKey(url))
                    ParsedLinks.Add(url, html);
                else if (string.IsNullOrEmpty(ParsedLinks[url]))
                    ParsedLinks[url] = html;

                if (ParsedLinks.Count <= MaxLinks && depth <= MaxDepth)
                {
                    foreach (string link in MatchMultiple(html, @"<a.*?href=(""|')(.*?)(""|').*?>(.*?)</a>", 2))
                    {
                        absoluteUrl = MakeAbsolute(link, this.Url);

                        // Checking whether we're in a same domain as well as whether this URL has already been processed
                        if (IsValidUrl(absoluteUrl) && IsSameDomain(link, this.Url) && !ParsedLinks.ContainsKey(absoluteUrl))
                        {
                            ParsedLinks.Add(absoluteUrl, string.Empty);

                            // Creating new node, notifying the clients
                            node = ToNode(absoluteUrl);

                            addTo.Add(node);
                            addedNodes.Add(node);

                            addedLinks++;
                        }

                        // Parsing up to [MaxLevelSize] on any level
                        if (addedLinks == MaxLevelSize)
                            break;
                    }

                    // Processing next level
                    foreach (SitemapNode addedNode in addedNodes)
                        ParseRecursive(addedNode.Url, addedNode.Children, depth + 1);
                }
            }
        }

        /// <summary>
        /// Fills node titles.
        /// </summary>
        private void FillTitles()
        {
            HashSet<string> emptyLinks = new HashSet<string>();

            foreach (string url in NodesToLinks.Keys)
            {
                if (ParsedLinks.ContainsKey(url))
                {
                    if (!string.IsNullOrEmpty(ParsedLinks[url]))
                    {
                        NodesToLinks[url].Title = MatchSingle(ParsedLinks[url], "<title>([^<]+)</title>", 1);

                        if (string.IsNullOrEmpty(NodesToLinks[url].Title))
                            emptyLinks.Add(url);
                    }
                    else
                        emptyLinks.Add(url);
                }
            }

            // Removing entries with empty HTML (no title)
            emptyLinks.ToList().ForEach(l => ParsedLinks.Remove(l));
        }

        /// <summary>
        /// Creates a sitemap node from the given URL.
        /// </summary>
        /// <param name="url">Page URL.</param>
        /// <returns>Sitemap node.</returns>
        private SitemapNode ToNode(string url) 
        {
            SitemapNode ret = new SitemapNode();

            ret.Url = url;
            ret.Title = string.Empty;
            ret.Children = new List<SitemapNode>();

            if (!NodesToLinks.ContainsKey(url))
                NodesToLinks.Add(url, ret);

            OnNodeCreated();

            return ret;
        }

        /// <summary>
        /// Occurs when new node has been added to the sitemap.
        /// </summary>
        private void OnNodeCreated()
        {
            int percentage = (int)Math.Floor(CurrentLinks * 100.0 / MaxLinks);

            // Don't report on 100% yet - we still nedd to fill out titles
            if (percentage > 100)
                percentage = 99;

            CurrentLinks++;
            Callback.OnProgress(OriginalUrl, percentage);
        }

        /// <summary>
        /// Downloads the contents of a given URL.
        /// </summary>
        /// <param name="url">URL to download.</param>
        /// <returns>URL contents.</returns>
        private string DownloadString(string url)
        {
            string ret = string.Empty, 
                contentType = string.Empty;

            if (IsValidUrl(url))
            {
                try
                {
                    using (var client = new ExtendedWebClient())
                    {
                        ret = client.DownloadString(url);
                        contentType = client.ResponseHeaders["Content-Type"];

                        // Only HTML documents can be processed
                        if (string.IsNullOrEmpty(contentType) || contentType.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) < 0)
                            ret = string.Empty;
                    }
                }
                catch (WebException) { }
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given URL is within the same domain as the base URL.
        /// </summary>
        /// <param name="url">Page URL.</param>
        /// <param name="baseUrl">Base URL.</param>
        /// <returns>Value indicating whether the given URL is within the same domain as the base URL.</returns>
        private bool IsSameDomain(string url, string baseUrl)
        {
            bool ret = !IsAbsolute(url);
            Uri baseUri = null, uri = null;

            if (!ret)
            {
                baseUri = new Uri(baseUrl, UriKind.Absolute);

                try
                {
                    uri = new Uri(AddProtocol(url), UriKind.Absolute);
                    ret = string.Compare(baseUri.Host, uri.Host, StringComparison.OrdinalIgnoreCase) == 0;
                }
                catch (UriFormatException) { ret = false; }
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given URL is valid as a sitemap node.
        /// </summary>
        /// <param name="url">Url.</param>
        /// <returns>Value indicating whether the given URL is valid as a sitemap node.</returns>
        private bool IsValidUrl(string url)
        {
            bool ret = true;
            string ext = string.Empty;
            string[] validExtensions = new[] { "aspx", "php", "html", "htm" };

            if (IsAbsolute(url))
                url = RemoveProtocol(url.TrimStart('/'));

            if (url.IndexOf('/') > 0)
            {
                ret = url.IndexOf("javascript:", StringComparison.OrdinalIgnoreCase) < 0;

                if (ret)
                {
                    ext = System.IO.Path.GetExtension(url).
                        Trim().Trim('.').ToLowerInvariant();

                    // Very naive but we can catch a lot of unwanted content (files, images)
                    ret = string.IsNullOrEmpty(ext) ||
                        validExtensions.Any(e => string.Compare(e, ext, StringComparison.OrdinalIgnoreCase) == 0);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given URL is absolute.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns>Value indicating whether the given URL is absolute.</returns>
        private bool IsAbsolute(string url)
        {
            return url.StartsWith("//") || HasProtocol(url);
        }

        /// <summary>
        /// Returns value indicating whether the given URL contains protocol.
        /// </summary>
        /// <param name="url">Page URL.</param>
        /// <returns>Value indicating whether the given URL contains protocol.</returns>
        private bool HasProtocol(string url)
        {
            int protocolEndIndex = url.IndexOf("://", StringComparison.OrdinalIgnoreCase);
            return protocolEndIndex > 0 && Regex.IsMatch(url.Substring(0, protocolEndIndex), "[a-z]+", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Adds the protocol (if needed) to a given URL.
        /// </summary>
        /// <param name="url">Page URL.</param>
        /// <returns>Page URL with the protocol added.</returns>
        private string AddProtocol(string url)
        {
            return HasProtocol(url) ? url : string.Format("http:{0}{1}", url.StartsWith("//") ? string.Empty : "//", url);
        }

        /// <summary>
        /// Removes the protocol (if needed) from a given URL.
        /// </summary>
        /// <param name="url">Page URL.</param>
        /// <returns>Page URL without the protocol added.</returns>
        private string RemoveProtocol(string url)
        {
            string ret = url, protocolEnd = "://";
            int protocolEndIndex = url.IndexOf(protocolEnd, StringComparison.OrdinalIgnoreCase);

            if (HasProtocol(ret))
                ret = ret.Substring(protocolEndIndex + protocolEnd.Length);

            return ret;
        }

        /// <summary>
        /// Makes the given URL absolute based on a given base URL.
        /// </summary>
        /// <param name="url">Page URL.</param>
        /// <param name="baseUrl">Base URL.</param>
        /// <returns>Absolute URL.</returns>
        private string MakeAbsolute(string url, string baseUrl)
        {
            return IsAbsolute(url) ? AddProtocol(url) : string.Format("{0}/{1}", baseUrl.TrimEnd('/'), url.TrimStart('/')).TrimEnd('/', '#');
        }

        /// <summary>
        /// Matches the given string against the given regular expression and returns the match (if any).
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="group">Capture group to use.</param>
        /// <returns>Match value.</returns>
        private string MatchSingle(string input, string pattern, int group = 0)
        {
            Match m = Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return m != null && m.Success ? m.Groups[group].Value.Trim() : string.Empty;
        }

        /// <summary>
        /// Matches the given string against the given regular expression and returns all matches (if any).
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="group">Capture group to use.</param>
        /// <returns>Match value.</returns>
        private IEnumerable<string> MatchMultiple(string input, string pattern, int group = 0)
        {
            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return matches != null ? matches.OfType<Match>().Select(m => m.Groups[group].Value.Trim()) : Enumerable.Empty<string>();
        }
    }
}
