using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Babalu.rProxy
{
    /// <summary>
    /// class to handle the management of the proxied services static responses (image, js, css, etc.)
    /// </summary>
    internal static class RequestCache
    {
        private static ReaderWriterLockSlim _requestCacheLock = new ReaderWriterLockSlim();
        private static Dictionary<string, byte[]> _reponseCache = new Dictionary<string, byte[]>();
        private static Dictionary<string, DateTime> _reponseAge = new Dictionary<string, DateTime>();

        /// <summary>
        /// add a item to the cache
        /// </summary>
        /// <param name="hostKey"></param>
        /// <param name="urlKey">the http request (first line of the request header) as a key to the cache</param>
        /// <param name="response">the data returned from the proxied server in the response (header and content)</param>
        /// <param name="seconds">the number of seconds that this item is considered fresh (-1 for forever)</param>
        public static void AddCache(string hostKey, string urlKey, byte[] response, int seconds)
        {
            _requestCacheLock.EnterUpgradeableReadLock();
            try
            {
                string key = string.Format("{0}/{1}", hostKey, urlKey).ToUpper();
                if (_reponseCache.ContainsKey(key) == false)
                {
                    _requestCacheLock.EnterWriteLock();
                    try
                    {
                        _reponseCache.Add(key, response);
                        _reponseAge.Add(key, (seconds == -1 ? DateTime.MaxValue : DateTime.Now.AddSeconds(seconds)));
                    }
                    finally
                    {
                        _requestCacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _requestCacheLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// see if a http request is in the server cache
        /// </summary>
        /// <param name="hostKey"></param>
        /// <param name="urlKey">the http request (first line of the request header) as a key to the cache</param>
        /// <returns>null if no cache hit else the cached response header and content</returns>
        public static byte[] GetCache(string hostKey, string urlKey)
        {
            _requestCacheLock.EnterUpgradeableReadLock();
            try
            {
                // all keys are based on the uppercase version of the key
                string key = string.Format("{0}/{1}", hostKey, urlKey).ToUpper();
                if (_reponseCache.ContainsKey(key))
                {
                    // if we found this key and it has not expired return it
                    if (_reponseAge[key] > DateTime.Now)
                        return _reponseCache[key];
                    else
                    {
                        _requestCacheLock.EnterWriteLock();
                        try
                        {
                            // the cache item has exired, remove it from the available cache items
                            _reponseAge.Remove(key);
                            _reponseCache.Remove(key);
                        }
                        finally
                        {
                            _requestCacheLock.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _requestCacheLock.ExitUpgradeableReadLock();
            }

            return null;
        }
    }
}
