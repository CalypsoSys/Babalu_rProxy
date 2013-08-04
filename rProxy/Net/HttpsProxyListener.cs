using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;

namespace Babalu.rProxy
{
    /// <summary>
    /// specific class to handle HTTPS requests
    /// </summary>
    internal class HttpsProxyListener : ProxyListener
    {
        private X509Certificate _cert;

        /// <summary>
        /// load the certicicate defined in the configuration file
        /// </summary>
        public HttpsProxyListener(BabaluProxiedServer proxiedServer,int port, string cert)
            : base(proxiedServer, port)
        {
            _cert = GetServerCert(cert);
        }

        /// <summary>
        /// Joes samples (so I remmeber where they are)
        /// "jsan17708317" 
        /// C:\Users\joe.schmitt\Desktop\Certs\JoeRootCertTest.cer
        /// </summary>
        /// <returns></returns>
        public static X509Certificate GetServerCert(string proxyCertificate)
        {
            if (File.Exists(proxyCertificate))
                return X509Certificate.CreateFromCertFile(proxyCertificate);
            else
            {
                X509Certificate cert = GetServerCert(StoreName.My, proxyCertificate);
                if (cert != null)
                    return cert;
                else
                    return GetServerCert(StoreName.AuthRoot, proxyCertificate);
            }
        }

        private static X509Certificate GetServerCert(StoreName storeName, string subject)
        {
            X509Store store = null;
            bool opened = false;
            try
            {
                store = new X509Store(storeName, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                opened = true;
                X509CertificateCollection cert = store.Certificates.Find(X509FindType.FindBySubjectName, subject, true);
                if (cert.Count > 0)
                    return cert[0];
            }
            finally
            {
                if ( opened ) 
                    store.Close();
            }

            return null;
        }

        /// <summary>
        /// authenticate and return a SSL stream for client processing
        /// </summary>
        /// <param name="realClientStream">the raw stream from the client</param>
        /// <returns>a SSL stream</returns>
        protected override Stream GetRealClientStream(NetworkStream realClientStream)
        {
            SslStream sslStream = null;
            try
            {
                sslStream = new SslStream(realClientStream, true);
                sslStream.AuthenticateAsServer(_cert, false, (SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls), false);
                return sslStream;
            }
            catch
            {
                Utility.SafeDispose( sslStream );
                throw;
            }
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
