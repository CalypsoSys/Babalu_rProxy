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
        private string _originalIp = string.Empty;
        private bool _started = false;
        public Form1()
        {
            InitializeComponent();

            if (PerformanceCounterCategory.Exists(BabaluCounterDescriptions.CounterCategory)) 
                _installCounterBtn.Text = "Remove Counters";

            var server = ExtensionConfig.Config.ProxiedServers.FirstOrDefault();
            if ( server != null )
            {
                _originalIp = server.ProxyIP;
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

            _logsLocationTxt.Text = ExtensionConfig.Config.BabaluServerConfiguration.LogsLocation;
            _logDebugChk.Checked = ExtensionConfig.Config.BabaluServerConfiguration.LogDebug;
            _logErrorsChk.Checked = ExtensionConfig.Config.BabaluServerConfiguration.LogErrors;
            _logInformationChk.Checked = ExtensionConfig.Config.BabaluServerConfiguration.LogInformation;
            _logRequestsChk.Checked = ExtensionConfig.Config.BabaluServerConfiguration.LogRequests;
            _enablePerfmonChk.Checked = ExtensionConfig.Config.BabaluServerConfiguration.EnablePerfmon;
            _eventLogChk.Checked = ExtensionConfig.Config.BabaluServerConfiguration.EnableEventLog;
        }

        private void _startBtn_Click(object sender, EventArgs e)
        {
            if (_started == false)
            {
                try
                {
                    BabaluServerConfiguration babaluServerConfiguration = new BabaluServerConfiguration()
                    {
                        LogsLocation = _logsLocationTxt.Text,
                        EnableEventLog = _eventLogChk.Checked,
                        EnablePerfmon = _enablePerfmonChk.Checked,
                        LogDebug = _logDebugChk.Checked,
                        LogErrors = _logErrorsChk.Checked,
                        LogInformation = _logInformationChk.Checked,
                        LogRequests = _logRequestsChk.Checked
                    };


                    BabaluProxiedServerConfiguration proxiedServer = new BabaluProxiedServerConfiguration()
                    {
                        ProxyIP = _proxyIPTxt.Text,
                        SupportGZip = _supportsGzipChk.Checked,
                        CacheContent = _cacheContentChk.Checked,
                        MaxQueueLength = Convert.ToInt32(_maxQueueLengthCtrl.Value)
                    };
                }
                catch (Exception excp)
                {
                    MessageBox.Show(this, excp.Message, "Exception Saving config");
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
