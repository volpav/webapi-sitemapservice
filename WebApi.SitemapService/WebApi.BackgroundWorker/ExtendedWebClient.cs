using System;
using System.Net;

namespace WebApi.BackgroundWorker
{
    /// <summary>
    /// Represents a web client.
    /// </summary>
    public class ExtendedWebClient : WebClient
    {
        /// <summary>
        /// Returns web request object for specified resource.
        /// </summary>
        /// <param name="uri">Resource URI.</param>
        /// <returns>Web request object.</returns>
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest ret = base.GetWebRequest(uri);

            // Avoiding waiting for too long
            ret.Timeout = 5 * 1000;

            return ret;
        }
    }
}
