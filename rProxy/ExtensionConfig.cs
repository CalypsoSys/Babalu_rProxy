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
    public class ExtensionConfig : IBabaluConfig, IBabaluExtension, IExternalMessageHandler
    {
        private static IBabaluExtension _extension = null;

        /// <summary>
        /// get config info
        /// </summary>
        public static BabaluConfiguration Config
        {
            get 
            {
                return BabaluConfigurationFactory.Instance; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        public static void StartBabalu(IBabaluExtension extension)
        {
            ExtensionConfig babaluConfig = new ExtensionConfig();
            _extension = extension ?? babaluConfig;
            LogFactory.Initialize(new Logger());
            BabaluConfigurationFactory.StartConfigWatcher();
            _extension.Initialize(babaluConfig);
            BabaluCounters.Initialize();
            ProxyListener.StartAll();
        }

        /// <summary>
        /// 
        /// </summary>
        public static IBabaluExtension BabaluExtension
        {
            get { return _extension; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopBabalu()
        {
            ProxyListener.StopAll();
            _extension.Terminate();
            BabaluCounters.Terminate();
            LogFactory.Stop();
        }

        private ExtensionConfig()
        {
        }

        string IBabaluConfig.ExternalData
        {
            get
            {
                return BabaluConfigurationFactory.Instance.ExternalData;
            }
            set
            {
                BabaluConfigurationFactory.Instance.ExternalData = value;
            }
        }

        bool IBabaluConfig.BypassProcessing
        {
            get { return BabaluConfigurationFactory.Instance.BypassProcessing; }
        }

        void IBabaluExtension.Initialize(IBabaluConfig config)
        {
        }

        void IBabaluExtension.Terminate()
        {
        }

        string IBabaluExtension.ProcessWcfMessage(Guid message, string data)
        {
            return string.Empty;
        }

        IExternalMessageHandler IBabaluExtension.MessageHandler(string serverType)
        {
            return this;
        }

        bool IExternalMessageHandler.OverrideResponseFromRequest(LogRequest log, string userName, string userAgent)
        {
            return false;
        }

        bool IExternalMessageHandler.OverrideResponseFromResponse(IBabaluMessage message, string userName)
        {
            return false;
        }

        string IExternalMessageHandler.ResponseCode
        {
            get { return null; }
        }

        byte[] IExternalMessageHandler.ResponseBuffer
        {
            get { return null; }
        }

        void IExternalMessageHandler.ProcessHeaderLineRequest(LogRequest log)
        {
        }

        void IExternalMessageHandler.ProcessHeaderLineResponse(LogRequest log)
        {
        }

        void IExternalMessageHandler.ProcessRequest(IBabaluMessage message, string userName)
        {
        }

        string IExternalMessageHandler.GetContentForLogging(IBabaluMessage message, out byte[] rawContent)
        {
            rawContent = null;
            return null;
        }

        byte[] IExternalMessageHandler.ProcessResponseContent(IBabaluMessage message, byte[] content)
        {
            return content;
        }

        string IExternalMessageHandler.ProcessUserNameAndPassword(string fullUnPw)
        {
            return fullUnPw;
        }

        bool IExternalMessageHandler.ReplaceOverride(IBabaluMessage message, List<byte> output, List<byte> input, ref int index)
        {
            return false;
        }

        void IExternalMessageHandler.ReplaceOutput(IBabaluMessage message, List<byte> output)
        {
        }
    }
}
