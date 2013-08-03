using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Babalu.rProxy;

namespace Babalu.rProxy
{
    /// <summary>
    /// abstract class for common worker thread funcitonality
    /// </summary>
    public abstract class BabaluWorkerThread
    {
        private Thread _threadHandler;
        private AutoResetEvent _threadEvent = new AutoResetEvent(false);

        /// <summary>
        /// the amount of time in mili-seconds to wait between operations
        /// </summary>
        protected abstract int DefaultWait { get; }
        /// <summary>
        /// the name of this thread
        /// </summary>
        protected abstract string ThreadName { get; }
        /// <summary>
        /// the work to do in this thread
        /// </summary>
        protected abstract void DoWork();
        /// <summary>
        /// when thread is done, cleanup anything
        /// </summary>
        protected abstract void Cleanup();

        /// <summary>
        /// start up the thread
        /// </summary>
        protected void Start()
        {
            LogFactory.LogInformation("Worker thread starting {0}", ThreadName);
            _threadHandler = new Thread(Worker);
            _threadHandler.Name = ThreadName;
            _threadHandler.IsBackground = true;
            _threadHandler.Start();
            LogFactory.LogInformation("Worker thread start called {0}", ThreadName);
        }

        /// <summary>
        /// stop the thread
        /// </summary>
        protected void Stop()
        {
            LogFactory.LogInformation("Worker thread stopping {0}", ThreadName);
            _threadEvent.Set();
            _threadHandler.Join();
            Cleanup();
            LogFactory.LogInformation("Worker thread stopped {0}", ThreadName);
        }

        /// <summary>
        /// do the actual work on a timed basis
        /// </summary>
        private void Worker()
        {
            LogFactory.LogInformation("Worker thread started {0}", ThreadName);
            while (_threadEvent.WaitOne(DefaultWait) == false)
            {
                try
                {
                    DoWork();
                }
                catch( Exception excp )
                {
                    LogFactory.LogException(excp, "Error in thread {0}", ThreadName);
                }
            }
        }
    }
}
