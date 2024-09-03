using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InteropTools.ShellPages.Gofly
{
    public static class WebUtils
    {
        private static Lazy<IWebProxy> proxy = new Lazy<IWebProxy>(() => null);

        public static IWebProxy Proxy
        {
            get { return WebUtils.proxy.Value; }
        }

        public static Task DownloadAsync(string requestUri, string filename)
        {
            if (requestUri == null)
                throw new ArgumentNullException("requestUri");

            return DownloadAsync(new Uri(requestUri), filename);
        }

        public static async Task DownloadAsync(Uri requestUri, string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            if (Proxy != null)
            {
                WebRequest.DefaultWebProxy = Proxy;
            }

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    using (
                        Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }
        }
    }
}
