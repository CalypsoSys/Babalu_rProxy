using Babalu.rProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TestApplication
{
    public partial class Form1 : Form
    {
        private bool _started = false;
        public Form1()
        {
            InitializeComponent();

            if (PerformanceCounterCategory.Exists(BabaluCounterDescriptions.CounterCategory)) 
                _installCounterBtn.Text = "Remove Counters";

            var server = ExtensionConfig.Config.ProxiedServers.FirstOrDefault();
            if ( server != null )
            {
                _proxyIPTxt.Text = server.ProxyIP;
                StringBuilder proxyPorts = new StringBuilder();
                foreach(int port in server.ProxyPorts.Keys)
                {
                    proxyPorts.AppendFormat("{0}|{1},", port, server.ProxyPorts[port]);
                }
                _proxyPortsTxt.Text = proxyPorts.ToString();

                StringBuilder proxiedServers = new StringBuilder();
                foreach (string serv in server.ProxiedServers.Keys)
                {
                    var tup = server.ProxiedServers[serv];
                    proxiedServers.AppendFormat("{0}|{1}|{2}|{3},", serv, tup.Item1, tup.Item2, tup.Item3);
                }
                _proxiedServerTxt.Text = proxiedServers.ToString();
                _supportsGzipChk.Checked = server.SupportGZip;
                _cacheContentChk.Checked = server.CacheContent;
                _maxQueueLengthCtrl.Value = server.MaxQueueLength;
            }

            _logsLocationTxt.Text = ExtensionConfig.Config.LogsLocation;
            _logDebugChk.Checked = ExtensionConfig.Config.LogDebug;
            _logErrorsChk.Checked = ExtensionConfig.Config.LogErrors;
            _logInformationChk.Checked = ExtensionConfig.Config.LogInformation;
            _logRequestsChk.Checked = ExtensionConfig.Config.LogRequests;
            _enablePerfmonChk.Checked = ExtensionConfig.Config.EnablePerfmon;
            _eventLogChk.Checked = ExtensionConfig.Config.EnableEventLog;
        }

        private void _startBtn_Click(object sender, EventArgs e)
        {
            if (_started == false)
            {
                try
                {
                    BabaluConfiguration babaluServerConfiguration = new BabaluConfiguration()
                    {
                        LogsLocation = _logsLocationTxt.Text,
                        EnableEventLog = _eventLogChk.Checked,
                        EnablePerfmon = _enablePerfmonChk.Checked,
                        LogDebug = _logDebugChk.Checked,
                        LogErrors = _logErrorsChk.Checked,
                        LogInformation = _logInformationChk.Checked,
                        LogRequests = _logRequestsChk.Checked,
                        ProxiedServers = new List<BabaluProxiedServer>()
                    };


                    babaluServerConfiguration.ProxiedServers.Add( new BabaluProxiedServer()
                            {
                                ProxyIP = _proxyIPTxt.Text,
                                ProxyPorts = LoadProxyPorts(_proxyPortsTxt.Text),
                                ProxiedServers = LoadProxiedServers(_proxiedServerTxt.Text),
                                SupportGZip = _supportsGzipChk.Checked,
                                CacheContent = _cacheContentChk.Checked,
                                MaxQueueLength = Convert.ToInt32(_maxQueueLengthCtrl.Value)
                            }
                    );

                    ExtensionConfig.SetConfiguration(babaluServerConfiguration);
                }
                catch (Exception excp)
                {
                    MessageBox.Show(this, excp.Message, "Exception setting config");
                }

                try
                {
                    ExtensionConfig.StartBabalu(null);
                    _started = true;
                    _startBtn.Text = "Stop";
                }
                catch (Exception excp)
                {
                    MessageBox.Show(this, excp.Message, "Exception Starting config");
                }
            }
            else
            {
                StopBabalu();
            }
        }

        private Dictionary<int, string> LoadProxyPorts(string proxyPorts)
        {
            Dictionary<int, string> portCerts = new Dictionary<int, string>();
            if (string.IsNullOrWhiteSpace(proxyPorts) == false)
            {
                string[] portCertEntries = proxyPorts.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string portCert in portCertEntries)
                {
                    string[] tokens = portCert.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Count() > 0)
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

            return portCerts;
        }

        private static Dictionary<string, Tuple<string, int, bool>> LoadProxiedServers(string proxiedServersPorts)
        {
            Dictionary<string, Tuple<string, int, bool>> servers = new Dictionary<string, Tuple<string, int, bool>>();

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


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopBabalu();
        }

        private void StopBabalu()
        {
            if (_started == true)
            {
                try
                {
                    ExtensionConfig.StopBabalu();
                    _started = false;
                    _startBtn.Text = "Start";
                }
                catch (Exception excp)
                {
                    MessageBox.Show(this, excp.Message, "Exception Stopping config");
                }
            }
        }

        private void _installCounterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (PerformanceCounterCategory.Exists(BabaluCounterDescriptions.CounterCategory))
                {
                    PerformanceCounterCategory.Delete(BabaluCounterDescriptions.CounterCategory);
                    _installCounterBtn.Text = "Install Counters";
                }
                else
                {
                    BabaluCounterDescriptions.InstallCounters();
                    _installCounterBtn.Text = "Remove Counters";
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(this, excp.Message, "Exception configuring counters");
            }
        }
    }
}
