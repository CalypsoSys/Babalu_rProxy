using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;
using System.IO;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Babalu.rProxy
{
    /// <summary>
    /// base class that handle tcp http connections
    /// </summary>
    internal abstract class ProxyListener
    {
        private static int InterruptedFunctionCall = 10004;
        private static int ExceptionSleep = 15000;
        private static List<ProxyListener> _proxyListeners = new List<ProxyListener>();

        private BabaluProxiedServer _proxiedServer;
        private Thread _handler;
        private TcpListener _listener;
        private bool _close = false;

        protected abstract Stream GetRealClientStream(NetworkStream realClientStream);
        protected abstract Stream GetProxyClientStream(NetworkStream proxyClientStream, string proxiedServer, bool ssl);

        /// <summary>
        /// try to start all the proxy listners configured
        /// </summary>
        /// <returns></returns>
        public static bool StartAll()
        {
            if ( BabaluConfigurationFactory.Instance.ProxiedServers != null )
            {
                foreach (BabaluProxiedServer proxiedServer in BabaluConfigurationFactory.Instance.ProxiedServers)
                {
                    try
                    {
                        Start(proxiedServer);
                    }
                    catch (Exception excp)
                    {
                        LogFactory.LogException(excp, "Could not start predefined Babalu listener: {0}", proxiedServer.ProxyIP);
                    }
                }
            }

            return (_proxyListeners.Count > 0);
        }

        /// <summary>
        /// start any listeners that are configured
        /// </summary>
        /// <returns></returns>
        private static void Start(BabaluProxiedServer proxiedServer)
        {
            List<ProxyListener> listeners = new List<ProxyListener>();
            foreach (int port in proxiedServer.ProxyPorts.Keys)
            {
                string cert = proxiedServer.ProxyPorts[port];
                if (string.IsNullOrWhiteSpace(cert))
                    listeners.Add(new HttpProxyListener(proxiedServer, port));
                else
                    listeners.Add(new HttpsProxyListener(proxiedServer, port, cert));
            }

            List<ProxyListener> started = new List<ProxyListener>();
            foreach(ProxyListener listener in listeners)
            {
                try
                {
                    listener.StartListner();
                    started.Add(listener);
                }
                catch
                {
                    foreach (ProxyListener startedListner in started)
                    {
                        try
                        {
                            startedListner.StopWork();
                        }
                        catch(Exception excp)
                        {
                            LogFactory.LogException(excp, "Unxpected error stopping pending Babalu Proxy Listener");
                        }
                    }
                    throw;
                }
            }

            foreach (ProxyListener listener in started)
            {
                _proxyListeners.Add(listener);
            }
        }

        /// <summary>
        /// stop all started listeners
        /// </summary>
        public static bool StopAll()
        {
            bool running = false;
            foreach (ProxyListener proxy in _proxyListeners)
            {
                proxy.StopWork();
                running = true;
            }
            _proxyListeners.Clear();

            return running;
        }

        protected ProxyListener(BabaluProxiedServer proxiedServer, int port )
        {
            _proxiedServer = proxiedServer;
            ProxyServerPort = port;
        }

        private void StartListner()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse(_proxiedServer.ProxyIP), ProxyServerPort);

                int workerThreads, portThreads;
                ThreadPool.GetMaxThreads(out workerThreads, out portThreads);
                LogFactory.LogInformation("Maximum worker threads=[{0}] Maximum completion port threads=[{1}]", workerThreads, portThreads);

                // start the listeners
                if ( _proxiedServer.MaxQueueLength > 0 )
                    _listener.Start(_proxiedServer.MaxQueueLength);
                else
                    _listener.Start();

                ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);
                LogFactory.LogInformation("Available worker threads=[{0}] Available completion port threads=[{1}]", workerThreads, portThreads);

                _handler = new Thread(StartWork);
                _handler.Start(_handler);
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Unxpected error starting Babalu Proxy Listener Exception");
                throw;
            }
        }

        protected BabaluProxiedServer ProxiedServer
        {
            get { return _proxiedServer; }
        }

        private int ProxyServerPort { get; set; }

        /// <summary>
        /// main processing loop of the tcp listener, wait for connections and hand off to thread for processing
        /// </summary>
        /// <param name="stateInfo">the thread this method is running on</param>
        private void StartWork(object stateInfo)
        {
            // run until we get a close request
            while (_close == false)
            {
                bool exception = false;
                try
                {
                    LogFactory.LogInformation("Waiting for incoming connections on port {0}", ProxyServerPort);
                    while (true)
                    {
                        // accept a client connection and pass the request off to a thread handler
                        TcpClient client = _listener.AcceptTcpClient();

                        BabaluCounters.IncrementPendingThread();
                        ThreadPool.QueueUserWorkItem(new WaitCallback(HandleAsyncConnection), client);
                    }
                }
                catch (SocketException socExcp)
                {
                    // if the service is not shutting down, log the exception
                    if (_close == false)
                    {
                        exception = true;
                        LogFactory.LogException(socExcp, "Unexpected Babalu Proxy Listener Socket Exception");
                    }
                    else if ( socExcp.ErrorCode != InterruptedFunctionCall )   // InterruptedFunctionCall is a expected service shut down request else unexpected
                        LogFactory.LogException(socExcp, "Unexpected Babalu Proxy Listener Socket close ({0} {1})", socExcp.ErrorCode, socExcp.Message);
                }
                catch (Exception excp)
                {
                    exception = true;
                    LogFactory.LogException(excp, "Unxpected Babalu Proxy Listener Exception");
                }

                // if a exception occurs and we are not shutting down wait "n" seconds and try again
                if (exception && _close == false)
                    Thread.Sleep(ExceptionSleep);
            }
            LogFactory.LogInformation("Stopping Babalu Proxy Listener {0}");
        }

        /// <summary>
        /// mark flag to stop listener, stop the listener and then wait for thread to terminate
        /// </summary>
        private void StopWork()
        {
            _close = true;
            _listener.Stop();
            _handler.Join();
        }

        /// <summary>
        ///  thread method to handle http request
        /// </summary>
        /// <param name="stateInfo">the TcpClient from the listener</param>
        private void HandleAsyncConnection(object stateInfo)
        {
            BabaluCounters.DecrementPendingThread();

            // initialize log information class
            LogRequest log = new LogRequest();

            Guid? activity = null;            
            try
            {
                // increment the gateway call count
                activity = BabaluCounters.IncrementAllRequest();

                using (TcpClient realClient = stateInfo as TcpClient)
                {
                    // get client information for logging
                    IPEndPoint localIp = realClient.Client.LocalEndPoint as IPEndPoint;
                    if (localIp != null)
                        log.ServerPort = localIp.Port.ToString();
                    IPEndPoint remoteIp = realClient.Client.RemoteEndPoint as IPEndPoint;
                    if (remoteIp != null)
                        log.ClientIp = remoteIp.Address.ToString();

                    // get client stream
                    using (NetworkStream realClientStream = realClient.GetStream())
                    {
                        // get specific implementation of client stream
                        using (Stream realClientStreamImpl = GetRealClientStream(realClientStream))
                        {
                            // process the client stream
                            IExternalMessageHandler messageHandler = ExtensionConfig.BabaluExtension.MessageHandler(_proxiedServer.ServerType);
                            ProxyRequestMessage request = new ProxyRequestMessage(messageHandler, _proxiedServer.SupportGZip, realClientStreamImpl, log);

                            if (messageHandler.OverrideResponseFromRequest(log, request.UserName, request.UserAgent))
                            {
                                realClientStreamImpl.Write(messageHandler.ResponseBuffer, 0, messageHandler.ResponseBuffer.Length);
                                log.ScStatus = messageHandler.ResponseCode;
                                log.ScSubstatus = "0";
                            }
                            else if (request.HasData && _proxiedServer.ProxiedServers.ContainsKey(request.Host.ToLower()))
                            {
                                Tuple<string, int, bool> proxied = _proxiedServer.ProxiedServers[request.Host.ToLower()];
                                string proxiedServer = proxied.Item1;
                                int proxiedPort = proxied.Item2;
                                bool proxiedSsl = proxied.Item3;

                                // see if this request is cached on the server
                                byte[] buffer = (_proxiedServer.CacheContent ? RequestCache.GetCache(request.Host, request.RequestUrl) : null);

                                if (buffer != null)
                                {
                                    // write out cache result, no need to ask proxied server for data
                                    realClientStreamImpl.Write(buffer, 0, buffer.Length);
                                    log.ScStatus = "200";
                                    log.ScSubstatus = "0";

                                    LogFactory.LogDebug("Cache hit {0}", request.RequestUrl);
                                    log.BabaluStatus = "Cache";
                                }
                                else
                                {
                                    // create a connection to the proxied server and pass client request to that server
                                    using (TcpClient proxyClient = new TcpClient(proxiedServer, proxiedPort))
                                    {
                                        // change all ip/dns information from proxy server to proxied server
                                        byte[] data = request.Tranform(request.Host, proxiedServer);

                                        // get proxied client stream
                                        using (NetworkStream proxyClientStream = proxyClient.GetStream())
                                        {
                                            // get specific implementation of proxied client stream
                                            using (Stream proxyClientImpl = GetProxyClientStream(proxyClientStream, proxiedServer, proxiedSsl))
                                            {
                                                // Send the message to the connected proxied server 
                                                proxyClientImpl.Write(data, 0, data.Length);

                                                // read the response from the proxied server
                                                ProxyResponseMessage response = new ProxyResponseMessage(messageHandler, _proxiedServer.SupportGZip, proxyClientImpl, log);
                                                if (response.HasData)
                                                {
                                                    if ( messageHandler.OverrideResponseFromResponse(response, request.UserName))
                                                    {
                                                        realClientStreamImpl.Write(messageHandler.ResponseBuffer, 0, messageHandler.ResponseBuffer.Length);
                                                        log.ScStatus = messageHandler.ResponseCode;
                                                        log.ScSubstatus = "0";

                                                        LogFactory.LogInformation("Response Overridden from Response: {0} User: {1}", log.ScStatus, request.UserName);
                                                    }
                                                    else
                                                    {
                                                        // change all ip/dns information from proxied server to proxy server - if provisional and it is a sync command special tranform processing
                                                        buffer = response.Tranform(proxiedServer, request.Host);
                                                        // write response back to the original client
                                                        realClientStreamImpl.Write(buffer, 0, buffer.Length);

                                                        // add request to server cache if possible
                                                        if (_proxiedServer.CacheContent)
                                                            response.ProcessCacheItem(request, buffer);
                                                    }
                                                }
                                                else
                                                    LogFactory.LogDebug("response has no data");

                                                ProxyMessage.RawLog(  request, response );
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                LogFactory.LogDebug("request has no data");
                        }
                    }
                }
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Processing to Request: {0}", log);
                log.ScSubstatus = "-1";
                BabaluCounters.IncrementException();
            }
            finally
            {
                LogFactory.LogRequest(_proxiedServer.ProxyIP, log);
                BabaluCounters.DecrementAllRequest(activity);
            }
        }

        /// <summary>
        /// authenticate and return a SSL stream from a proxied server 
        /// </summary>
        /// <param name="proxyClientStream">the raw socket stream from the server</param>
        /// <param name="proxiedServer"></param>
        /// <returns></returns>
        protected Stream GetProxySslClientStream(NetworkStream proxyClientStream, string proxiedServer)
        {
            SslStream sslStream = null;
            try
            {
                sslStream = new SslStream(proxyClientStream, true);
                sslStream.AuthenticateAsClient(proxiedServer);
                return sslStream;
            }
            catch
            {
                Utility.SafeDispose( sslStream );
                throw;
            }
        }
    }
}
