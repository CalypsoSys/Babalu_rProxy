using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using Babalu.rProxy;

namespace Babalu.rProxy
{
    /// <summary>
    /// Installer class to create/remove performance counters
    /// </summary>
    [RunInstaller(true)]
    public class PerformanceCounterInstaller : Installer
    {
        /// <summary>
        /// default constructor for service installer
        /// </summary>
        public PerformanceCounterInstaller()
        {
        }

        /// <summary>
        /// install any application specific items
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            SetupPerfmonCounters(Context);
        }

        /// <summary>
        /// uninstall any application specific items
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            if (PerformanceCounterCategory.Exists(BabaluCounterDescriptions.CounterCategory))
            {
                Context.LogMessage("Perfmon Babalu counters uninstaller begins");
                PerformanceCounterCategory.Delete(BabaluCounterDescriptions.CounterCategory);
                Context.LogMessage("Perfmon Babalu counters uninstaller ends");
            }
        }

        /// <summary>
        /// setup the Babalu perfmon counters
        /// </summary>
        /// <param name="context"></param>
        public static void SetupPerfmonCounters(InstallContext context)
        {
            context.LogMessage("Perfmon Babalu counters installer begins");
#if !DEBUG
            if (PerformanceCounterCategory.Exists(BabaluCounterDescriptions.CounterCategory))
                PerformanceCounterCategory.Delete(BabaluCounterDescriptions.CounterCategory);
#endif
            BabaluCounterDescriptions.InstallCounters();

            context.LogMessage("Perfmon Babalu counters installer ends");
        }

    }
}
