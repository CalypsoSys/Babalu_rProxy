using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babalu.rProxy
{
    /// <summary>
    /// interface to the logging instance
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// start the logger
        /// </summary>
        void Start();

        /// <summary>
        /// stop the logger
        /// </summary>
        void Stop();

        /// <summary>
        /// log this request
        /// </summary>
        /// <param name="proxyIP"></param>
        /// <param name="request"></param>
        void LogRequest(string proxyIP, LogRequest request);

        /// <summary>
        /// log this exception
        /// </summary>
        /// <param name="excp"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogException(Exception excp, string message, params object[] args);

        /// <summary>
        /// log informational items
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogInformation(string message, params object[] args);

        /// <summary>
        /// log debug items
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogDebug(string message, params object[] args);
    }
}
