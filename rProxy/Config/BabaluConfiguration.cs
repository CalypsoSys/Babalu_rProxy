using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babalu.rProxy
{
    /// <summary>
    /// Babalu server configuration and Proxied server settings
    /// </summary>
    [DataContract]
    public class BabaluConfiguration
    {
        /// <summary>
        /// the Babalu servers configuration
        /// </summary>
        [DataMember]
        public BabaluServerConfiguration BabaluServerConfiguration
        {
            get; set;
        }

        /// <summary>
        /// the Babalu Proxied servers
        /// </summary>
        [DataMember]
        public List<BabaluProxiedServerConfiguration> ProxiedServers
        {
            get; set;
        }
    }
}
