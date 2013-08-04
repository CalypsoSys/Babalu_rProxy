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
        ///  Where to log 
        /// </summary>
        [DataMember]
        public string LogsLocation
        {
            get; set;
        }

        /// <summary>
        ///  does the service log all requests (IIS like log) 
        /// </summary>
        [DataMember]
        public bool LogRequests
        {
            get; set;
        }

        /// <summary>
        /// does the service log unexpected exceptions/errors
        /// </summary>
        [DataMember]
        public bool LogErrors
        {
            get; set;
        }

        /// <summary>
        /// does the service log informational items
        /// </summary>
        [DataMember]
        public bool LogInformation
        {
            get; set;
        }
        
        /// <summary>
        /// does the service log developer/debugging information
        /// </summary>
        [DataMember]
        public bool LogDebug
        {
            get; set;
        }

        /// <summary>
        /// does the service log errors to the event log
        /// </summary>
        [DataMember]
        public bool EnableEventLog
        {
            get; set;
        }

        /// <summary>
        /// does the service log perfmon counter
        /// </summary>
        [DataMember]
        public bool EnablePerfmon
        {
            get; set;
        }

        /// <summary>
        ///  Overrided all rule processing
        /// </summary>
        [DataMember]
        public bool BypassProcessing
        {
            get; set;
        }

        /// <summary>
        /// a string extensions can set
        /// </summary>
        [DataMember]
        public string ExternalData
        {
            get; set;
        }

        /// <summary>
        /// the Babalu Proxied servers
        /// </summary>
        [DataMember]
        public List<BabaluProxiedServer> ProxiedServers
        {
            get; set;
        }
    }
}
