using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Babalu.rProxy
{
    /// <summary>
    /// class to handle the HTTP response message
    /// </summary>
    internal class ProxyResponseMessage : ProxyMessage
    {
        private const int _readBufferSize = 20480;

        /// <summary>
        /// contructor processes the HTTP response data
        /// </summary>
        /// <param name="messageHandler">external message handler</param>
        /// <param name="supportsGZip">are we supporting GZip compression</param>
        /// <param name="input"></param>
        /// <param name="log"></param>
        public ProxyResponseMessage(IExternalMessageHandler messageHandler, bool supportsGZip, Stream input, LogRequest log)
            : base(messageHandler, supportsGZip, input, log, false)
        {
            if (FirstHeaderLine != null)
            {
                // parse the status result for the response
                string[] parts = FirstHeaderLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    log.ScStatus = parts[1];
                    log.ScSubstatus = "0";

                    messageHandler.ProcessHeaderLineResponse(log);
                    int status;
                    if ( int.TryParse(log.ScStatus, out status) == false && log.ScStatus.StartsWith("html", StringComparison.InvariantCultureIgnoreCase) )
                        log.ScStatus = "200";
                }
                else
                {
                    // default to a 200 "Success" status
                    log.ScStatus = "200";
                    log.ScSubstatus = "0";
                }

            }
        }

        /// <summary>
        /// put the request into the cache if possible
        /// </summary>
        /// <param name="comms"></param>
        /// <param name="buffer"></param>
        public void ProcessCacheItem(ProxyRequestMessage comms, byte[] buffer)
        {
            int age = CacheAge;
            if (age != 0)
                RequestCache.AddCache(comms.Host, comms.RequestUrl, buffer, age);
        }

        /// <summary>
        /// this is the amount of data we want to read at a time
        /// </summary>
        public override int BufferSize
        {
            get { return _readBufferSize; }
        }

        /// <summary>
        /// not special processing for request header data
        /// </summary>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected override int SpecialReplace(List<byte> input, int index, List<byte> output)
        {
            return index;
        }
    }
}
