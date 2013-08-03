using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babalu.rProxy
{
    /// <summary>
    /// class to saftely process temp log queue for performance
    /// </summary>
    internal class LogQueue
    {
        private object _logSync = new object();
        private Queue<string> _logQueue = new Queue<string>();

        /// <summary>
        /// add a empty string
        /// </summary>
        public void Enqueue()
        {
            Enqueue(string.Empty);
        }

        /// <summary>
        /// add a item to the queue
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Enqueue(string message, params object[] args)
        {
            lock (_logSync)
                _logQueue.Enqueue(string.Format(message, args));
        }

        /// <summary>
        /// return all the items in the queue and clear the queue
        /// </summary>
        /// <returns></returns>
        public string[] Dequeue()
        {
            string [] data;
            lock( _logSync )
            {
                data = _logQueue.ToArray();
                _logQueue.Clear();
            }
            return data;
        }

        /// <summary>
        /// clear all log messages
        /// </summary>
        public void Clear()
        {
            lock (_logSync)
            {
                _logQueue.Clear();
            }
        }

        /// <summary>
        /// copy all log messages without clearing queue
        /// </summary>
        public string[] Copy()
        {
            lock (_logSync)
            {
                return _logQueue.ToArray();
            }
        }
    }
}
