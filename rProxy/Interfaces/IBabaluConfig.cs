using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Babalu.rProxy
{
    /// <summary>
    /// interface to the babalu configuration 
    /// </summary>
    public interface IBabaluConfig
    {
        /// <summary>
        /// get/set the external config data
        /// </summary>
        /// <returns></returns>
        string ExternalData { get; set; }

        /// <summary>
        /// bypass processing?
        /// </summary>
        bool BypassProcessing { get; }
    }
}
