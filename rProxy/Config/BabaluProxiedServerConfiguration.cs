using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babalu.rProxy
{
    /// <summary>
    /// Babalu rProxy set configuration Proxied server settings
    /// </summary>
    [DataContract]
    public class BabaluProxiedServerConfiguration
    {
        /// <summary>
        /// the local network address that the tcp listener should use to listen on
        /// </summary>
        [DataMember]
        public string ProxyIP
        {
            get; set;
        }

        /// <summary>
        /// dictionary of ports and cert names supported
        /// </summary>
        [DataMember]
        public Dictionary<int, string> ProxyPorts
        {
            get; set;
        }

        /// <summary>
        /// does the service allow HTTP content to be compressed with gzip (content type)
        /// </summary>
        [DataMember]
        public bool SupportGZip
        {
            get; set;
        }

        /// <summary>
        /// does the service allow for the caching of server content so we do not have to call the proxied server on all requests
        /// </summary>
        [DataMember]
        public bool CacheContent
        {
            get; set;
        }

        /// <summary>
        /// the The maximum requested length of the pending connections queue.
        /// </summary>
        [DataMember]
        public int MaxQueueLength
        {
            get; set;
        }

        /// <summary>
        /// the The maximum requested length of the pending connections queue.
        /// </summary>
        [DataMember]
        public Dictionary<string, Tuple<string, int, bool>> ProxiedServers
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ServerType
        {
            get; set;
        }
    }
}
