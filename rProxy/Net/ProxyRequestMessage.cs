using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using System.Xml;

namespace Babalu.rProxy
{
    /// <summary>
    /// class to handle HTTP request message
    /// </summary>
    internal class ProxyRequestMessage : ProxyMessage
    {
        private const int _readBufferSize = 10480;
        private static string _identity = " identity\r";
        private static string _encoding = "Accept-Encoding:";

        /// <summary>
        /// contructor processes the HTTP request data
        /// </summary>
        /// <param name="messageHandler">external message handler</param>
        /// <param name="supportsGZip">are we supporting GZip compression</param>
        /// <param name="input"></param>
        /// <param name="log"></param>
        public ProxyRequestMessage(IExternalMessageHandler messageHandler, bool supportsGZip, Stream input, LogRequest log)
            : base(messageHandler, supportsGZip, input, log, true)
        {
            if (FirstHeaderLine != null)
            {
                // parse the first line to get header information
                string[] parts = FirstHeaderLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    // method is the first token
                    log.CsMethod = parts[0];
                    if (parts.Length > 1)
                    {
                        // parse the query string
                        int index = parts[1].IndexOf('?');
                        if (index == -1)
                            log.CsUriStem = parts[1];
                        else
                        {
                            log.CsUriStem = parts[1].Substring(0, index);
                            log.CsUriQuery = parts[1].Substring(index + 1);
                        }

                        messageHandler.ProcessHeaderLineRequest(log);

                        // get the HTTP version if available
                        if (parts.Length > 2)
                            log.CsVersion = parts[2];
                    }
                }

                if (Authorization != null)
                {
                    // get header authorization infromation
                    int index = Authorization.IndexOf(_basicAuthorization);
                    if (index == 0)
                    {
                        // if basic authentication, get un/domain
                        log.CsUsername = ProcessUserNameAndPassword( Authorization.Substring(index + _basicAuthorization.Length), true, log.CsUsername);
                    }
                }
            }

            // must happen before settings code code below as it depends on username
            UserName = log.CsUsername;

            messageHandler.ProcessRequest(this, UserName);
        }

        /// <summary>
        /// get the basic authentication username if provided
        /// </summary>
        public string UserName
        {
            get; private set;
        }

        /// <summary>
        /// return the entire first line of the header
        /// </summary>
        public string RequestUrl
        {
            get { return FirstHeaderLine; }
        }

        /// <summary>
        /// this is the amount of data we want to read at a time
        /// </summary>
        public override int BufferSize
        {
            get { return _readBufferSize; }
        }

        /// <summary>
        /// special processing for gzip and ???, since ??? is ???? do not use gzip
        /// </summary>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected override int SpecialReplace(List<byte> input, int index, List<byte> output)
        {
            // replace the Accept-Encoding: token value with "identity" so the server does not think this is a gzip request 
            if (input[index] == _encoding[0] && Utility.Match(input, index, _encoding))
            {
                foreach (char c in _encoding)
                    output.Add((byte)c);
                foreach (char c in _identity)
                    output.Add((byte)c);
                while (input[index] != '\n')
                {
                    ++index;
                }
            }

            return index;
        }
    }
}
