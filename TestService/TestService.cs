using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Net;
using Babalu.rProxy;

namespace Babalu.rProxy
{
    /// <summary>
    /// main service for Babalu rProxy service
    /// </summary>
    public partial class TestService : ServiceBase
    {
        /// <summary>
        /// inititialize the service
        /// </summary>
        public TestService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// handle the service start action
        /// Start the tcp listeners and the config watcher
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
#if DEBUG
            // time to attach in debug mode
            Thread.Sleep(15000);
#endif
            // start logger first as others may depend on it
            try
            {
                ExtensionConfig.StartBabalu(null);
            }
            catch
            {
#if DEBUG
                Debugger.Launch();
#endif
                throw;
            }
        }

        /// <summary>
        /// handle the service stop action
        /// stop the tcp listeners
        /// </summary>
        protected override void OnStop()
        {
            ExtensionConfig.StopBabalu();
        }
    }
}
