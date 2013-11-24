using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WebApi.BackgroundWorker.Host
{
    /// <summary>
    /// Represents a program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Represents an entry point of an application.
        /// </summary>
        /// <param name="args">Arguments.</param>
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(WebApi.BackgroundWorker.SitemapService)))
            {
                host.Open();

                Console.WriteLine();

                foreach (ServiceEndpoint sep in host.Description.Endpoints)
                {
                    Console.WriteLine("Open endpoint: {0} ({1})",
                        sep.Address, sep.Binding.Name);
                }

                Console.WriteLine();

                Console.WriteLine("Enter any key to close the host...");
                Console.ReadLine();

                host.Close();
            }
        }
    }
}
