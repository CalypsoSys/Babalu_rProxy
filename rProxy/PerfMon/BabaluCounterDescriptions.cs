using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Babalu.rProxy
{
    /// <summary>
    /// Babalu performance counter descriptions
    /// </summary>
    public static class BabaluCounterDescriptions
    {
        /// <summary>
        /// all requests
        /// </summary>
        public const string CurrentRequests = "All Current Requests";
        /// <summary>
        /// total call time
        /// </summary>
        public const string TotalCallTime = "Total Call Time";
        /// <summary>
        /// exceptions per minute
        /// </summary>
        public const string ExceptionsPerMinute = "Exceptions per Minute";
        /// <summary>
        /// pending thread requests
        /// </summary>
        public const string PendingThreadRequest = "Pending Thread Requests";
        /// <summary>
        /// requests per second
        /// </summary>
        public const string RequestsPerSecond = "Requests per Second";

        private static string[] BabaluCounters = { CurrentRequests, TotalCallTime, ExceptionsPerMinute, 
                                                    PendingThreadRequest, RequestsPerSecond };
        /// <summary>
        /// counter category name
        /// </summary>
        public const string CounterCategory = "Babalu rProxy Server";
        /// <summary>
        /// counter category description
        /// </summary>
        public const string CategoryDescription = "Babalu rProxy Server Performance Counters.";

        /// <summary>
        /// install thge perfmon counters
        /// </summary>
        public static void InstallCounters()
        {
            if (!PerformanceCounterCategory.Exists(BabaluCounterDescriptions.CounterCategory))
            {
                //Create the collection that will hold 
                // the data for the counters we are
                // creating.
                CounterCreationDataCollection counterData = new CounterCreationDataCollection();

                //Create the CreationData object. 
                foreach (string counter in BabaluCounterDescriptions.BabaluCounters)
                {
                    CounterCreationData BabaluCounter = new CounterCreationData();

                    // Set the counter's type to NumberOfItems32
                    BabaluCounter.CounterType = PerformanceCounterType.NumberOfItems32;

                    //Set the counter's name
                    BabaluCounter.CounterName = counter;

                    //Add the CreationData object to our
                    //collection
                    counterData.Add(BabaluCounter);
                }

                //Create the counter in the system using the collection. 
                PerformanceCounterCategory.Create(BabaluCounterDescriptions.CounterCategory, BabaluCounterDescriptions.CategoryDescription,
                                    PerformanceCounterCategoryType.SingleInstance, counterData);
            }
        }
    }
}
