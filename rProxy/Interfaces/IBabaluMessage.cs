using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Babalu.rProxy
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBabaluMessage
    {
        /// <summary>
        /// 
        /// </summary>
        bool SupportsGzip { get; }

        /// <summary>
        /// 
        /// </summary>
        string ContentEncoding { get; }

        /// <summary>
        /// 
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// 
        /// </summary>
        byte[] Content { get; }

        /// <summary>
        /// 
        /// </summary>
        byte[] RawRead { get; }

        /// <summary>
        /// 
        /// </summary>
        int BufferSize { get; }

        /// <summary>
        /// 
        /// </summary>
        string BasicAuthorizationToken { get; }

        /// <summary>
        /// 
        /// </summary>
        string UserQuery { get; }
    }
}
