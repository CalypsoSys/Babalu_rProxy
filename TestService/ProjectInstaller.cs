using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;

namespace Babalu.rProxy
{
    /// <summary>
    /// basic installer class for service so installutil for a install program can install
    /// </summary>
#if DEBUG
    [RunInstaller(true)]
#else
    [RunInstaller(false)]
#endif
    public partial class ProjectInstaller : Installer
    {
        /// <summary>
        /// default constructor for service installer
        /// </summary>
        public ProjectInstaller()
        {
#if DEBUG
            InitializeComponent();
#endif
        }

        /// <summary>
        /// install any application specific items
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
#if DEBUG
            base.Install(stateSaver);
#endif
        }

        /// <summary>
        /// uninstall any application specific items
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
#if DEBUG
            base.Uninstall(savedState);
#endif
        }
    }
}
