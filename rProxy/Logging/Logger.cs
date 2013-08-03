using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Babalu.rProxy
{
    internal class Logger : ILogger
    {
        private const string _softwareFormatLine = "#Software: Babalu rProxy Service {0}";
        private const string _versionFormatLine = "#Version: {0}";
        private const string _dateFormatLine = "#Date: {0}";
        private const string _siteName = "Babalu_rProxy";

        private const string _fieldLine = "#Fields:";
        private const string _fieldDate = "date";
        private const string _fieldTime = "time";
        private const string _fieldServerSitename = "s-sitename";
        private const string _fieldServerComputername = "s-computername";
        private const string _fieldServerIp = "s-ip";
        private const string _fieldCsMethod = "cs-method";
        private const string _fieldCsUriStem = "cs-uri-stem";
        private const string _fieldCsUriQuery = "cs-uri-query";
        private const string _fieldServerPort = "s-port";
        private const string _fieldCsUsername = "cs-username";
        private const string _fieldClientIp = "c-ip";
        private const string _fieldCsVersion = "cs-version";
        private const string _fieldCsUserAgent = "cs(User-Agent)";
        private const string _fieldScStatus = "sc-status";
        private const string _fieldScSubstatus = "sc-substatus";
        private const string _fieldDeviceType = "device-type";
        private const string _fieldBabaluStatus = "babalu-status";

        private static string[] _fields = { _fieldDate, _fieldTime, _fieldServerSitename, _fieldServerComputername, _fieldServerIp, _fieldCsMethod, _fieldCsUriStem, _fieldCsUriQuery,
                                            _fieldServerPort, _fieldCsUsername, _fieldClientIp, _fieldCsVersion, _fieldCsUserAgent, _fieldScStatus, _fieldScSubstatus, 
                                            _fieldDeviceType, _fieldBabaluStatus };

        private const int _defaultWait = 1000;
        private ILogger _me;
        private Thread _logHandler;
        private AutoResetEvent _logEvent = new AutoResetEvent(false);
        private LogQueue _resultsLogQueue = new LogQueue();
        private LogQueue _exceptionLogQueue = new LogQueue();
        private LogQueue _informationLogQueue = new LogQueue();
        private LogQueue _debugLogQueue = new LogQueue();

        private string _softwareLine;
        private string _versionLine;
        private string _computerName;
        private string _lastLogDirectory;
        private DateTime _lastRequestDate = DateTime.Now.Date;
        private DateTime _lastErrorDate = DateTime.Now.Date;
        private DateTime _lastInfoDate = DateTime.Now.Date;
        private DateTime _lastDebugDate = DateTime.Now.Date;

        public Logger()
        {
            _me = this;
        }

        /// <summary>
        /// initialize commen logging variables and start log thread
        /// </summary>
        void ILogger.Start()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            _softwareLine = string.Format(_softwareFormatLine, version);
            _versionLine = string.Format(_versionFormatLine, version);
            _computerName = Environment.MachineName;

            _logHandler = new Thread(LogWriter);
            _logHandler.Start();
        }

        /// <summary>
        /// stop the log thread
        /// </summary>
        void ILogger.Stop()
        {
            _logEvent.Set();
            _logHandler.Join();
        }

        /// <summary>
        /// thread method to wait "n" milliseconds and then process logs
        /// </summary>
        private void LogWriter()
        {
            try
            {
                TextWriter logRequestStream = null;
                TextWriter logExceptionStream = null;
                TextWriter logInformationStream = null;
                TextWriter logDebugStream = null;
                while (_logEvent.WaitOne(_defaultWait) == false)
                {
                    bool create = false;
                    if (_lastLogDirectory != BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogsLocation)
                    {
                        create = true;
                        _lastLogDirectory = BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogsLocation;
                        try
                        {
                            Directory.CreateDirectory(_lastLogDirectory);
                        }
                        catch
                        {
                        }
                    }

                    if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogRequests)
                    {
                        logRequestStream = LogRequestStream(logRequestStream, create, ref _lastRequestDate);
                        WriteLog(_resultsLogQueue, logRequestStream );
                    }
                    _resultsLogQueue.Clear();

                    // *********************************************************************************************
                    // WriteEvents must be called before WriteLog call with _exceptionLogQueue as WriteLog will clear the queue
                    WriteEvents();
                    if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogErrors)
                    {
                        logExceptionStream = LogStream(logExceptionStream, "babalu_rproxy_error", create, ref _lastErrorDate);
                        WriteLog(_exceptionLogQueue, logExceptionStream);
                    }
                    _exceptionLogQueue.Clear();
                    // *********************************************************************************************

                    if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogInformation)
                    {
                        logInformationStream = LogStream(logInformationStream, "babalu_rproxy_info", create, ref _lastInfoDate);
                        WriteLog(_informationLogQueue, logInformationStream);
                    }
                    _informationLogQueue.Clear();

                    if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogDebug)
                    {
                        logDebugStream = LogStream(logDebugStream, "babalu_rproxy_debug", create, ref _lastDebugDate);
                        WriteLog(_debugLogQueue, logDebugStream);
                    }
                    _debugLogQueue.Clear();
                }
            }
            catch
            {
                // do no harm in logging
            }
        }

        private void WriteEvents()
        {
            try
            {
                if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.EnableEventLog)
                {
                    string[] messages = _exceptionLogQueue.Copy();
                    foreach( string message in messages )
                        EventLog.WriteEntry("Babalu Logging", message, EventLogEntryType.Error);
                }
            }
            catch
            {
                // TODO - what should we do here??????
            }
        }

        /// <summary>
        /// write data from the queue to the stream
        /// </summary>
        /// <param name="logQueue"></param>
        /// <param name="writer"></param>
        private void WriteLog(LogQueue logQueue, TextWriter writer)
        {
            try
            {
                string []data = logQueue.Dequeue();
                if (data.Length > 0)
                {
                    foreach (string output in data)
                        writer.WriteLine(output);
                    writer.Flush();
                }
            }
            catch
            {
                // do no harm in logging
            }
        }

        /// <summary>
        /// IIS like logging factility
        /// </summary>
        /// <param name="proxyIP">The IP of this proxy server</param>
        /// <param name="request">request specific information</param>
        void ILogger.LogRequest(string proxyIP, LogRequest request)
        {
            if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogRequests)
            {
                try
                {
                    StringBuilder output = new StringBuilder(256);
                    foreach (string field in _fields)
                    {
                        string value = null;
                        switch (field)
                        {
                            case _fieldDate:
                                value = DateTime.Now.ToString("yyyy-MM-dd");
                                break;
                            case _fieldTime:
                                value = DateTime.Now.ToString("HH:mm:ss");
                                break;
                            case _fieldServerSitename:
                                value = _siteName;
                                break;
                            case _fieldServerComputername:
                                value = _computerName;
                                break;
                            case _fieldServerIp:
                                value = proxyIP;
                                break;
                            case _fieldCsMethod:
                                value = request.CsMethod;
                                break;
                            case _fieldCsUriStem:
                                value = request.CsUriStem;
                                break;
                            case _fieldCsUriQuery:
                                value = request.CsUriQuery;
                                break;
                            case _fieldServerPort:
                                value = request.ServerPort;
                                break;
                            case _fieldCsUsername:
                                value = request.CsUsername;
                                break;
                            case _fieldClientIp:
                                value = request.ClientIp;
                                break;
                            case _fieldCsVersion:
                                value = request.CsVersion;
                                break;
                            case _fieldCsUserAgent:
                                value = request.CsUserAgent;
                                break;
                            case _fieldScStatus:
                                value = request.ScStatus;
                                break;
                            case _fieldScSubstatus:
                                value = request.ScSubstatus;
                                break;
                            case _fieldDeviceType:
                                value = request.ExternalInfo;
                                break;
                            case _fieldBabaluStatus:
                                value = request.BabaluStatus;
                                break;
                        }

                        if (string.IsNullOrEmpty(value))
                            value = "-";
                        output.AppendFormat("{0} ", value.Replace(" ", ""));
                    }

                    _resultsLogQueue.Enqueue(output.ToString());
                }
                catch (Exception excp)
                {
                    // eat any exception as we do not want to do any harm when logging
                    _me.LogException(excp, "Writting to Request log");
                }
            }
        }

        /// <summary>
        /// initialize request/IIS log file, if the day has changed create a new log file (just like IIS), and output standard header information
        /// </summary>
        private TextWriter LogRequestStream(TextWriter requestStream, bool create, ref DateTime lastDate)
        {
            try
            {
                requestStream = LogStream(requestStream, "babalu_rproxy", ref create, ref lastDate);
                if (create && requestStream != null)
                {
                    requestStream.WriteLine();
                    requestStream.WriteLine(_softwareLine);
                    requestStream.WriteLine(_versionLine);
                    requestStream.WriteLine(_dateFormatLine, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    requestStream.Write(_fieldLine);
                    foreach (string field in _fields)
                        requestStream.Write(" {0}", field);
                    requestStream.WriteLine();
                    requestStream.Flush();
                }

                return requestStream;
            }
            catch
            {
                // do no harm in logging
            }

            return null;
        }

        private TextWriter LogStream(TextWriter requestStream, string fileName, bool create, ref DateTime lastDate)
        {
            return LogStream(requestStream, fileName, ref create, ref lastDate);
        }

        private TextWriter LogStream(TextWriter requestStream, string fileName, ref bool create, ref DateTime lastDate)
        {
            create = create || (lastDate != DateTime.Now.Date);
            if (create)
                lastDate = DateTime.Now.Date;
            requestStream = LogStream(requestStream, string.Format("{0}_{1}.log", fileName, lastDate.ToString("yyyyMMdd")), ref create);
            return requestStream;
        }

        private TextWriter LogStream(TextWriter requestStream, string fileName, ref bool create)
        {
            try
            {
                if (create || requestStream == null)
                {
                    Utility.SafeDispose( requestStream );

                    requestStream = new StreamWriter(new FileStream(Path.Combine(_lastLogDirectory, fileName), FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write, FileShare.Read));

                    create = true;
                }

                return requestStream;
            }
            catch
            {
                // do no harm in logging
            }

            return null;
        }

        /// <summary>
        /// log all unexpected exception encountered
        /// </summary>
        /// <param name="excp">the exception to log</param>
        /// <param name="message">developer specified message</param>
        /// <param name="args">arguments to format in the message parameter</param>
        void ILogger.LogException(Exception excp, string message, params object[] args)
        {
            if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogErrors)
            {
                try
                {
                    StringBuilder output = new StringBuilder();
                    output.AppendFormat("{0}\t{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), string.Format(message, args));

                    // start at 2, since we want to start at the 3rd position
                    for (int i = 2; excp != null; i++)
                    {
                        string indent = string.Empty.PadLeft(i, '\t');
                        output.AppendFormat("{0}Exception: {1}\r\n", indent, excp.Message);
                        output.AppendFormat("{0}Stack Trace: {1}\r\n", indent, excp.StackTrace.Replace("\r\n", string.Format("\r\n{0}", indent)));
                        excp = excp.InnerException;
                    }
                    output.AppendLine();

                    _exceptionLogQueue.Enqueue(output.ToString());
                }
                catch
                {
                    // do no harm in logging
                }
            }

            _me.LogInformation(string.Format("Exception: {0}", message), args);
        }

        /// <summary>
        /// log informational items to the Babalu_rProxy_INFO.log log
        /// </summary>
        /// <param name="message">developer specified message</param>
        /// <param name="args">arguments to format in the message parameter</param>
        void ILogger.LogInformation(string message, params object[] args)
        {
            if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogInformation)
                LogGenetic(false, _informationLogQueue, message, args);
        }

        /// <summary>
        /// log debugging items to the Babalu_rProxy_DEBUG.log log
        /// </summary>
        /// <param name="message">developer specified message</param>
        /// <param name="args">arguments to format in the message parameter</param>
        void ILogger.LogDebug(string message, params object[] args)
        {
            if (BabaluConfigurationFactory.Instance.BabaluServerConfiguration.LogDebug)
                LogGenetic(true, _debugLogQueue, message, args);
        }

        /// <summary>
        /// generic log writer to handle both information and debug logging
        /// </summary>
        /// <param name="debug">flag to determine log type</param>
        /// <param name="logQueue">the log queue to add to</param>
        /// <param name="message">developer specified message</param>
        /// <param name="args">arguments to format in the message parameter</param>
        private void LogGenetic(bool debug, LogQueue logQueue, string message, params object[] args)
        {
            try
            {
                string logMessage = string.Format(message, args);
                if ( debug )
                    logQueue.Enqueue();
                logQueue.Enqueue("{0}{1}{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), (debug ? "\r\n" : " "), logMessage);
                if ( debug )
                    logQueue.Enqueue();
            }
            catch
            {
                // do no harm in logging
            }
        }
    }
}
