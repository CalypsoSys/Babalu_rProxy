using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babalu.rProxy
{
    /// <summary>
    /// common static logger class
    /// </summary>
    public static class LogFactory
    {
        private static ILogger _logger = null;

        /// <summary>
        /// initialize the logger of the user of this assembly
        /// </summary>
        /// <param name="logger"></param>
        public static void Initialize(ILogger logger)
        {
            _logger = logger;
            _logger.Start();
        }

        /// <summary>
        /// stop the logger
        /// </summary>
        public static void Stop()
        {
            if (_logger != null)
                _logger.Stop();
        }

        /// <summary>
        /// log this request
        /// </summary>
        /// <param name="proxyIP"></param>
        /// <param name="request"></param>
        public static void LogRequest(string proxyIP, LogRequest request)
        {
            if (_logger != null)
                _logger.LogRequest(proxyIP, request);
        }

        /// <summary>
        /// log this exception
        /// </summary>
        /// <param name="excp"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogException(Exception excp, string message, params object[] args)
        {
            if (_logger != null)
                _logger.LogException(excp, message, args);
        }

        /// <summary>
        /// log informational items
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInformation(string message, params object[] args)
        {
            if (_logger != null)
                _logger.LogInformation(message, args);
        }

        /// <summary>
        /// log debug items
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogDebug(string message, params object[] args)
        {
            if (_logger != null)
                _logger.LogDebug(message, args);
        }
    }
}
