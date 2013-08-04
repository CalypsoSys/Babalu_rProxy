using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;
using Babalu.rProxy;

namespace Babalu.rProxy
{
    /// <summary>
    /// manager configuration all file access
    /// </summary>
    internal static class BabaluConfigurationFactory
    {
        private const string _babaluFolderName = "Babalu_rProxy";
        private const string _appSettingsSectionName = "appSettings";
        private const string _logsLocationKey = "LogsLocation";
        private const string _logRequestsKey = "LogRequests";
        private const string _logErrorsKey = "LogErrors";
        private const string _logInformationKey = "LogInformation";
        private const string _logDebugKey = "LogDebug";
        private const string _enablePerfmonLogKey = "EnablePerfmon";
        private const string _enableEventLogKey = "EnableEventLog";
        private const string _bypassProcessingKey = "BypassProcessing";

        private const string _proxyIPKey = "ProxyIP{0}";
        private const string _proxyPortsKey = "ProxyPorts{0}";
        private const string _supportGZipKey = "SupportGZip{0}";
        private const string _cacheContentKey = "CacheContent{0}";
        private const string _maxQueueLengthKey = "MaxQueueLength{0}";
        private const string _proxiedServersKey = "ProxiedServers{0}";
        private const string _serverTypeKey = "ServerType{0}";
        

        private static FileSystemWatcher _fileWatcher = null;
        private static DateTime _lastWriteTime = DateTime.MinValue;

        private static ReaderWriterLockSlim _configLock = new ReaderWriterLockSlim();
        private static BabaluConfiguration _configuration = null;

        public static BabaluConfiguration Instance
        {
            get
            {
                _configLock.EnterReadLock();
                try
                {
                    if (_configuration == null)
                    {
                        _configuration = LoadConfiguration();
                    }

                    return _configuration;
                }
                finally
                {
                    _configLock.ExitReadLock();
                }
            }
            private set
            {
                SetConfiguration(value);
            }
        }

        public static void SetConfiguration(BabaluConfiguration config)
        {
            _configLock.EnterWriteLock();
            try
            {
                bool running = ProxyListener.StopAll();
                _configuration = config;
                if (running && _configuration == null)
                    _configuration = LoadConfiguration();
                if ( running )
                    ProxyListener.StartAll();
            }
            finally
            {
                _configLock.ExitWriteLock();
            }
        }

        private static BabaluConfiguration LoadConfiguration()
        {
            BabaluConfiguration configuration = new BabaluConfiguration()
            {
                LogsLocation = LoadLogsLocation(),
                LogRequests = BoolConfigKey(_logRequestsKey, null, true),
                LogErrors = BoolConfigKey(_logErrorsKey, null, true),
                LogInformation = BoolConfigKey(_logInformationKey, null, true),
                LogDebug = BoolConfigKey(_logDebugKey, null, false),
                EnablePerfmon = BoolConfigKey(_enablePerfmonLogKey, null, true),
                EnableEventLog = BoolConfigKey(_enableEventLogKey, null, false),
                BypassProcessing = BoolConfigKey(_bypassProcessingKey, null, false)
            };

            configuration.ProxiedServers = new List<BabaluProxiedServer>();
            string proxyIP;
            for (int i = 1; (proxyIP = StringConfigKey(_proxyIPKey, i, null)) != null; i++)
            {
                configuration.ProxiedServers.Add(new BabaluProxiedServer()
                        {
                            ProxyIP = proxyIP,
                            ProxyPorts = LoadProxyPorts(i),
                            SupportGZip = BoolConfigKey(_supportGZipKey, i, true),
                            CacheContent = BoolConfigKey(_cacheContentKey, i, true),
                            MaxQueueLength = IntConfigKey(_maxQueueLengthKey, i, 0),
                            ProxiedServers = LoadProxiedServers(i),
                            ServerType = StringConfigKey(_serverTypeKey, i, null)
                        }
                    );
            }

            return configuration;
        }

        /// <summary>
        /// check to see if the cert exists
        /// </summary>
        /// <param name="certificate"></param>
        private static void TestCertificate(string certificate)
        {
            // check to see if the cert exists
            try
            {
                if (HttpsProxyListener.GetServerCert(certificate) == null)
                    throw new Exception();
            }
            catch (Exception excp)
            {
                throw new Exception(string.Format("Invalid proxy certificate identifier: {0}", certificate), excp);
            }
        }

        /// <summary>
        /// start watching the applications configuation file
        /// </summary>
        public static void StartConfigWatcher()
        {
            if (_fileWatcher == null)
            {
                LogFactory.LogInformation("Staring configuation watcher");
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                _fileWatcher = new FileSystemWatcher();
                _fileWatcher.Path = Path.GetDirectoryName(config.FilePath);
                _fileWatcher.Filter = Path.GetFileName(config.FilePath);
                _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _fileWatcher.Changed += OnChange;
                _fileWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// when a change occurs to the configuration file, update the config section and reload types
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnChange(object source, FileSystemEventArgs e)
        {
            try
            {
                // for some reason this is fired twice, check date to make sure we only update once
                DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
                if (_lastWriteTime != lastWriteTime)
                {
                    ConfigurationManager.RefreshSection(BabaluConfigurationFactory._appSettingsSectionName);

                    SetConfiguration(null);
                    _lastWriteTime = lastWriteTime;
                }
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Exception after configuration file changed");
            }
        }

        /// <summary>
        /// get the location to write log files
        /// </summary>
        private static string LoadLogsLocation()
        {
            string logsLocation = StringConfigKey(_logsLocationKey, null, null);

            try
            {
                if (string.IsNullOrEmpty(logsLocation))
                    logsLocation = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), _babaluFolderName), "Logs");
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Could not create a default log file location");
            }

            return logsLocation;
        }

        private static Dictionary<int, string> LoadProxyPorts(int i)
        {
            Dictionary<int, string> portCerts = new Dictionary<int, string>();
            string proxyPorts = StringConfigKey(_proxyPortsKey, i, null);
            if (string.IsNullOrWhiteSpace(proxyPorts) == false)
            {
                string[] portCertEntries = proxyPorts.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string portCert in portCertEntries)
                {
                    string[] tokens = portCert.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if ( tokens.Count() > 0 )
                    {
                        int port;
                        if (int.TryParse(tokens[0], out port))
                        {
                            string cert = null;
                            if (tokens.Count() > 1 && string.IsNullOrWhiteSpace(tokens[1]) == false)
                                cert = tokens[1];
                            portCerts.Add(port, cert);
                        }
                    }
                }
            }

            if (portCerts.Count == 0)
                portCerts.Add(80, null);

            return portCerts;
        }

        private static Dictionary<string, Tuple<string, int, bool>> LoadProxiedServers(int i)
        {
            Dictionary<string, Tuple<string, int, bool>> servers = new Dictionary<string, Tuple<string, int, bool>>();

            string proxiedServersPorts = StringConfigKey(_proxiedServersKey, i, null);
            if (string.IsNullOrWhiteSpace(proxiedServersPorts) == false)
            {
                string[] serverPortMaps = proxiedServersPorts.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string serverPortMap in serverPortMaps)
                {
                    string[] tokens = serverPortMap.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Count() == 4 && string.IsNullOrWhiteSpace(tokens[0]) == false && string.IsNullOrWhiteSpace(tokens[1]) == false)
                    {
                        int port;
                        if (int.TryParse(tokens[2], out port) == false)
                            port = 80;
                        bool ssl;
                        if (bool.TryParse(tokens[3], out ssl) == false)
                            ssl = false;

                        servers.Add(tokens[0].ToLower(), Tuple.Create<string, int, bool>(tokens[1], port, ssl));
                    }
                }
            }

            return servers;
        }

        private static string StringConfigKey(string key, object param, string defaultValue)
        {
            try
            {
                return ConfigurationManager.AppSettings[string.Format(key, param)];
            }
            catch
            {
                return defaultValue;
            }
        }

        private static bool BoolConfigKey(string key, object param, bool defaultValue)
        {
            try
            {
                return bool.Parse(StringConfigKey(key, param, null));
            }
            catch
            {
                return defaultValue;
            }
        }

        private static int IntConfigKey(string key, object param, int defaultValue)
        {
            try
            {
                return int.Parse(StringConfigKey(key, param, null));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
