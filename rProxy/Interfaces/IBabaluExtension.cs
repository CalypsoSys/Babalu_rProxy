using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Babalu.rProxy
{
    /// <summary>
    /// interface from a external extension
    /// </summary>
    public interface IBabaluExtension
    {
        /// <summary>
        /// initialize this external library
        /// </summary>
        /// <param name="config"></param>
        void Initialize(IBabaluConfig config);

        /// <summary>
        /// terminate this external library
        /// </summary>
        void Terminate();

        /// <summary>
        /// process this WCF message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        string ProcessWcfMessage(Guid message, string data);

        /// <summary>
        /// process the request
        /// </summary>
        /// <returns>returns false if no other processing is required</returns>
        IExternalMessageHandler MessageHandler(string serverType);
    }
}
