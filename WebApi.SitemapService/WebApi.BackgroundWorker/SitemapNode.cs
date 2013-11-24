using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebApi.BackgroundWorker
{
    /// <summary>
    /// Represents a sitemap node.
    /// </summary>
    [DataContract]
    public class SitemapNode
    {
        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        [DataMember]
        public IList<SitemapNode> Children { get; set; }

        /// <summary>
        /// Gets or sets the node title.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the node URL.
        /// </summary>
        [DataMember]
        public string Url { get; set; }
    }
}
