using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babalu.rProxy
{
    /// <summary>
    /// class to gather request specific information to be logged into the IIS Like log
    /// </summary>
    public class LogRequest
    {
        /// <summary>
        /// the https method
        /// </summary>
        public string CsMethod { get; set; }
        /// <summary>
        /// the http stem uri
        /// </summary>
        public string CsUriStem { get; set; }
        /// <summary>
        /// the http url query string
        /// </summary>
        public string CsUriQuery { get; set; }
        /// <summary>
        /// the http server port
        /// </summary>
        public string ServerPort { get; set; }
        /// <summary>
        /// the http user name of this request
        /// </summary>
        public string CsUsername { get; set; }
        /// <summary>
        /// the http clients IP address
        /// </summary>
        public string ClientIp { get; set; }
        /// <summary>
        /// the http client version #
        /// </summary>
        public string CsVersion { get; set; }
        /// <summary>
        /// the http client user agent used
        /// </summary>
        public string CsUserAgent { get; set; }
        /// <summary>
        /// the http server status for this request
        /// </summary>
        public string ScStatus { get; set; }
        /// <summary>
        /// the http server sub-status for this request
        /// </summary>
        public string ScSubstatus { get; set; }
        /// <summary>
        /// the device type that this request if from
        /// </summary>
        public string ExternalInfo { get; set; }
        /// <summary>
        /// the Babalu blocking status for this call
        /// </summary>
        public string BabaluStatus { get; set; }

        /// <summary>
        /// dump contents of class to string for diagnosict purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                        "CsMethod={0}, CsUriStem={1}, CsUriQuery={2}, ServerPort={3}, CsUsername={4}, ClientIp={5}, CsVersion={6}, CsUserAgent={7}, ScStatus={8}, ScSubstatus={9}, ExtnernalInfo={10}, BabaluStatus={11}",
                        CsMethod, CsUriStem, CsUriQuery, ServerPort, CsUsername, ClientIp, CsVersion, CsUserAgent, ScStatus, ScSubstatus, ExternalInfo, BabaluStatus);

        }
    }
}
