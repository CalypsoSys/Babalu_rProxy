using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babalu.rProxy
{
    /// <summary>
    /// class to track positions of various items in a HTTP header
    /// </summary>
    internal class HeaderPositions
    {
        public HeaderPositions()
        {
            FirstLineEnd = -1;
            HeaderEnd = -1;
            Host = -1;
            ContentType = -1;
            ContentLength = -1;
            ContentEncoding = -1;
            TransferEncoding = -1;
            CacheControl = -1;
            UserAgent = -1;
            Authorization = -1;
        }

        public int FirstLineEnd { get; set; }
        public int HeaderEnd { get; set; }
        public int Host { get; set; }
        public int ContentType { get; set; }
        public int ContentLength { get; set; }
        public int ContentEncoding { get; set; }
        public int TransferEncoding { get; set; }
        public int CacheControl { get; set; }
        public int UserAgent { get; set; }
        public int Authorization { get; set; }
    }
}
