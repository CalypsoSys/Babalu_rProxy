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
    public interface IExternalMessageHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool OverrideResponseFromRequest(LogRequest log, string userName, string userAgent);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool OverrideResponseFromResponse(IBabaluMessage message, string userName);

        /// <summary>
        /// 
        /// </summary>
        string ResponseCode { get; }

        /// <summary>
        /// 
        /// </summary>
        byte[] ResponseBuffer { get; }

        /// <summary>
        /// 
        /// </summary>
        void ProcessHeaderLineRequest(LogRequest log);

        /// <summary>
        /// 
        /// </summary>
        void ProcessHeaderLineResponse(LogRequest log);

        /// <summary>
        /// 
        /// </summary>
        void ProcessRequest(IBabaluMessage message, string userName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetContentForLogging(IBabaluMessage message, out byte[] rawContent);

        /// <summary>
        /// 
        /// </summary>
        byte[] ProcessResponseContent(IBabaluMessage message, byte[] content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullUnPw"></param>
        /// <returns></returns>
        string ProcessUserNameAndPassword(string fullUnPw);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool ReplaceOverride(IBabaluMessage message, List<byte> output, List<byte> input, ref int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        void ReplaceOutput(IBabaluMessage message, List<byte> output);

    }
}
