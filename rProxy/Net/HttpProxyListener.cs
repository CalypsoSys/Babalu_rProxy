using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.IO;

namespace Babalu.rProxy
{
    /// <summary>
    /// specific class to handle HTTP requests
    /// </summary>
    internal class HttpProxyListener : ProxyListener
    {
        public HttpProxyListener(BabaluProxiedServerConfiguration proxiedServer, int overridePort)
            : base(proxiedServer, overridePort)
        {
        }

        /// <summary>
        /// just return the same stream as not additional processing needs to take place
        /// </summary>
        /// <param name="realClientStream">the raw stream from the client</param>
        /// <returns></returns>
        protected override Stream GetRealClientStream(NetworkStream realClientStream)
        {
            return realClientStream;
        }

        /// <summary>
        /// return either a HTTP or HTTPS stream based on the proxied server's configuration
        /// </summary>
        /// <param name="proxyClientStream">the client to the proxied server</param>
        /// <param name="proxiedServer"></param>
        /// <param name="ssl"></param>
        /// <returns></returns>
        protected override Stream GetProxyClientStream(NetworkStream proxyClientStream, string proxiedServer, bool ssl)
        {
            if (ssl)
                return GetProxySslClientStream(proxyClientStream, proxiedServer);
            else
                return proxyClientStream;
        }
    }
}
