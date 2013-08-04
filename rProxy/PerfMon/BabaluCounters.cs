using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Babalu.rProxy
{
    /// <summary>
    /// perfmon Babalu counter information
    /// </summary>
    internal sealed class BabaluCounters : BabaluWorkerThread
    {
        private const int _populateInterval = 30;
        private const string _perfmonThreadName = "Babalu_rProxyPerfmonThread";
        private static volatile BabaluCounters _instance;
        private static object _syncRoot = new object();

        private PerformanceCounter _pendingThreadsCounter;
        private PerformanceCounter _currentAllRequestsCounter;
        private PerformanceCounter _totalCallTimeCounter;
        private PerformanceCounter _exceptionsPerMinuteCounter;
        private PerformanceCounter _requestsPerSecondCounter;

        private long _countPendingThreads = 0;
        private long _countCurrentRequests = 0;
        private Dictionary<Guid, long> _callTimes = new Dictionary<Guid, long>();
        private long _countExceptionsPerMinute = 0;
        private long _lastExceptionsPerMinute = 0;
        private long _countRequestPerMinute = 0;

        /// <summary>
        /// Initialize the performance counter objects and thread for the Babalu rProxy
        /// </summary>
        public static void Initialize()
        {
            try
            {
                LogFactory.LogInformation("Babalu Counters initializing");
                if (BabaluConfigurationFactory.Instance.EnablePerfmon)
                {
                    if (_instance == null)
                    {
                        lock (_syncRoot)
                        {
                            if (_instance == null)
                            {
                                _instance = new BabaluCounters();
                                _instance.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception excp)
            {
                _instance = null;
                LogFactory.LogException(excp, "Babalu perfmon counters initialization failed");
            }
        }

        /// <summary>
        /// check to see if we shoudl start or stop logging of performance counters
        /// </summary>
        public static void Refresh()
        {
            if (BabaluConfigurationFactory.Instance.EnablePerfmon)
                BabaluCounters.Initialize();
            else
                BabaluCounters.Terminate();
        }

        /// <summary>
        /// get the currently configured Babalu counter instance
        /// </summary>
        private static BabaluCounters Instance
        {
            get
            {
                if (_instance == null)
                    Initialize();
                return _instance;
            }
        }

        /// <summary>
        /// increment request counter
        /// </summary>
        public static void IncrementPendingThread()
        {
            try
            {
                if (BabaluConfigurationFactory.Instance.EnablePerfmon)
                    Instance.IncrementPendingThreads();
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Babalu perfmon counters increment pending threads");
            }
        }

        /// <summary>
        /// Decrement request counter
        /// </summary>
        public static void DecrementPendingThread()
        {
            try
            {
                if (BabaluConfigurationFactory.Instance.EnablePerfmon)
                    Instance.DecrementPendingThreads();
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Babalu perfmon counters decrement pending threads");
            }
        }


        /// <summary>
        /// increment request counter
        /// </summary>
        public static Guid? IncrementAllRequest()
        {
            try
            {
                if (BabaluConfigurationFactory.Instance.EnablePerfmon) 
                {
                    Guid activity = Guid.NewGuid();
                    Instance.IncrementAllRequests(activity);
                    return activity;
                }
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Babalu perfmon counters increment request");
            }

            return null;
        }

        /// <summary>
        /// Decrement request counter
        /// </summary>
        public static void DecrementAllRequest(Guid? guid)
        {
            try
            {
                if (BabaluConfigurationFactory.Instance.EnablePerfmon && guid.HasValue)
                    Instance.DecrementAllRequests(guid.Value);
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Babalu perfmon counters decrement request");
            }
        }

        /// <summary>
        /// increment the exception count
        /// </summary>
        public static void IncrementException()
        {
            try
            {
                if (BabaluConfigurationFactory.Instance.EnablePerfmon)
                    Instance.IncrementExceptions();
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Babalu perfmon exception increment");
            }
        }

        /// <summary>
        /// get the perfmon stats for the MAC through WPF, not recommeneded but if all else fails...
        /// </summary>
        /// <returns></returns>
        public static long[] GetPerfmonStatistics(string[] stats)
        {
            if (BabaluConfigurationFactory.Instance.EnablePerfmon)
                return Instance.GetPerfmonStats(stats);
            else
                return null;
        }

        /// <summary>
        ///  terminate the population of performance counters
        /// </summary>
        public static void Terminate()
        {
            try
            {
                if (_instance != null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance != null)
                        {
                                _instance.Stop();
                                _instance = null;
                        }
                    }
                }
            }
            catch (Exception excp)
            {
                _instance = null;
                LogFactory.LogException(excp, "Babalu perfmon counters termination failed");
            }
        }

        /// <summary>
        /// initialize the performance counters
        /// </summary>
        private BabaluCounters()
        {
            SetCounters();
        }

        protected override int DefaultWait
        {
            get { return _populateInterval * 1000; }
        }

        protected override string ThreadName
        {
            get { return _perfmonThreadName; }
        }

        /// <summary>
        /// thread to run the population of performance counters 
        /// upate every _populateInterval seconds
        /// </summary>
        protected override void DoWork()
        {
            if (BabaluConfigurationFactory.Instance.EnablePerfmon)
            {
                _totalCallTimeCounter.RawValue = GetTotalCallTime(_callTimes);
                if (_lastExceptionsPerMinute >= 60)
                {
                    _exceptionsPerMinuteCounter.RawValue = Interlocked.Exchange(ref _countExceptionsPerMinute, 0);
                    _lastExceptionsPerMinute = 0;
                }
                else
                {
                    _exceptionsPerMinuteCounter.RawValue = Interlocked.Read(ref _countExceptionsPerMinute);
                    _lastExceptionsPerMinute += _populateInterval;
                }

                _requestsPerSecondCounter.RawValue = Interlocked.Exchange(ref _countRequestPerMinute, 0) / _populateInterval;
            }
        }

        /// <summary>
        ///  cleanup managed objects
        /// </summary>
        protected override void Cleanup()
        {
            try
            {
                Utility.SafeDispose( _pendingThreadsCounter );
                Utility.SafeDispose( _currentAllRequestsCounter );
                Utility.SafeDispose( _totalCallTimeCounter );
                Utility.SafeDispose( _exceptionsPerMinuteCounter );
                Utility.SafeDispose( _requestsPerSecondCounter );
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "Babalu perfmon counters managed cleanup failed");
            }
        }

        /// <summary>
        /// initialize the performance counters
        /// </summary>
        private void SetCounters()
        {
            _pendingThreadsCounter = new PerformanceCounter(BabaluCounterDescriptions.CounterCategory, BabaluCounterDescriptions.PendingThreadRequest, false);
            _pendingThreadsCounter.RawValue = 0;
            _currentAllRequestsCounter = new PerformanceCounter(BabaluCounterDescriptions.CounterCategory, BabaluCounterDescriptions.CurrentRequests, false);
            _currentAllRequestsCounter.RawValue = 0;
            _totalCallTimeCounter = new PerformanceCounter(BabaluCounterDescriptions.CounterCategory, BabaluCounterDescriptions.TotalCallTime, false);
            _totalCallTimeCounter.RawValue = 0;
            _exceptionsPerMinuteCounter = new PerformanceCounter(BabaluCounterDescriptions.CounterCategory, BabaluCounterDescriptions.ExceptionsPerMinute, false);
            _exceptionsPerMinuteCounter.RawValue = 0;
            _requestsPerSecondCounter = new PerformanceCounter(BabaluCounterDescriptions.CounterCategory, BabaluCounterDescriptions.RequestsPerSecond, false);
            _requestsPerSecondCounter.RawValue = 0;
        }

        /// <summary>
        /// increment the pending thread count
        /// </summary>
        private void IncrementPendingThreads()
        {
            _pendingThreadsCounter.RawValue = Interlocked.Increment(ref _countPendingThreads);
        }

        /// <summary>
        /// decrement the pending thread count
        /// </summary>
        private void DecrementPendingThreads()
        {
            _pendingThreadsCounter.RawValue = Interlocked.Decrement(ref _countPendingThreads);
        }

        /// <summary>
        /// increment the gate way call based on the activity id
        /// </summary>
        /// <param name="guid"></param>
        private void IncrementAllRequests(Guid guid)
        {
            _currentAllRequestsCounter.RawValue = Interlocked.Increment(ref _countCurrentRequests);
            Interlocked.Increment(ref _countRequestPerMinute);
            IncrementCallTime(guid, _callTimes);
        }

        /// <summary>
        /// decrement the gate way call based on the activity id
        /// </summary>
        /// <param name="guid"></param>
        private void DecrementAllRequests(Guid guid)
        {
            _currentAllRequestsCounter.RawValue = Interlocked.Decrement(ref _countCurrentRequests);
            DecrementCallTime(guid, _callTimes);
        }

        /// <summary>
        /// increment the gate way exception count
        /// </summary>
        private void IncrementExceptions()
        {
            Interlocked.Increment(ref _countExceptionsPerMinute);
        }

        private long[] GetPerfmonStats(string[] stats)
        {
            long[] statValues = new long[stats.Length];

            for (int i = 0; i < stats.Length; i++)
            {
                long value = -1;
                switch (stats[i])
                {
                    case BabaluCounterDescriptions.CurrentRequests:
                        value = _currentAllRequestsCounter.RawValue;
                        break;
                    case BabaluCounterDescriptions.TotalCallTime:
                        value = _totalCallTimeCounter.RawValue;
                        break;
                    case BabaluCounterDescriptions.ExceptionsPerMinute:
                        value = _exceptionsPerMinuteCounter.RawValue;
                        break;
                    case BabaluCounterDescriptions.PendingThreadRequest:
                        value = _pendingThreadsCounter.RawValue;
                        break;
                    case BabaluCounterDescriptions.RequestsPerSecond:
                        value = _requestsPerSecondCounter.RawValue;
                        break;
                }
                statValues[i] = value;
            }
            return statValues;
        }

        /// <summary>
        /// add this guid to the list of outstanding requests and mark it's start time
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="ticks"></param>
        private void IncrementCallTime(Guid guid, Dictionary<Guid, long> ticks)
        {
            long now = DateTime.Now.Ticks;
            lock (ticks)
            {
                if (ticks.ContainsKey(guid) == false)
                {
                    ticks.Add(guid, now);
                }
            }
        }

        /// <summary>
        /// add this guid to the list of outstanding requests
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="ticks"></param>
        private void DecrementCallTime(Guid guid, Dictionary<Guid, long> ticks)
        {
            lock (ticks)
            {
                if (ticks.ContainsKey(guid))
                    ticks.Remove(guid);
            }
        }

        /// <summary>
        /// get the total amount of call time in all active requests
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private long GetTotalCallTime(Dictionary<Guid, long> ticks)
        {
            StringBuilder longRunningEntries = new StringBuilder();
            long now = DateTime.Now.Ticks;
            lock (ticks)
            {
                long total = 0;
                foreach (Guid guid in ticks.Keys)
                {
                    long start = ticks[guid];
                    double seconds = TimeSpan.FromTicks(now - start).TotalSeconds;
                    if (seconds > (3 * 60))
                        longRunningEntries.AppendLine(guid.ToString());
                    total += Convert.ToInt64(seconds);
                }

                // perhaps log log running processes
                //if (longRunningEntries.Length > 0)
                //    ;

                return total;
            }
        }
    }
}
