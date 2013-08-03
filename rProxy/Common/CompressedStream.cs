using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace Babalu.rProxy
{
    /// <summary>
    /// 
    /// </summary>
    public class CompressedStream : IDisposable 
    {
        private static string _contentIsDeflateToken = "deflate";
        private static string _contentIsGZipToken = "gzip";

        private GZipStream _gzipStream;
        private DeflateStream _deflateStream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public static bool IsCompressed(string contentEncoding)
        {
            return (contentEncoding != null &&
                    (contentEncoding.IndexOf(_contentIsGZipToken, StringComparison.InvariantCultureIgnoreCase) != -1 ||
                        contentEncoding.IndexOf(_contentIsDeflateToken, StringComparison.InvariantCultureIgnoreCase) != -1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="contentEncoding"></param>
        /// <param name="rawRead"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static byte[] DeCompressContent(byte[] content, string contentEncoding, byte[] rawRead, int bufferSize)
        {
            using (MemoryStream inputStream = new MemoryStream(content))
            {
                // read in and convert gzipped content compressed content and then fix it up
                using (CompressedStream deCompressStream = new CompressedStream(contentEncoding, inputStream, CompressionMode.Decompress))
                {
                    using (MemoryStream outputSteam = new MemoryStream())
                    {
                        int bytes;
                        do
                        {
                            bytes = deCompressStream.Read(rawRead, 0, bufferSize);
                            if (bytes != 0)
                                outputSteam.Write(rawRead, 0, bytes);
                        }
                        while (bytes != 0);
                        outputSteam.Flush();

                        return outputSteam.ToArray();
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentEncoding"></param>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        public CompressedStream(string contentEncoding, Stream stream, CompressionMode mode)
        {
            if (contentEncoding.IndexOf(_contentIsGZipToken, StringComparison.InvariantCultureIgnoreCase) != -1)
                _gzipStream = new GZipStream(stream, mode);
            else if (contentEncoding != null && contentEncoding.IndexOf(_contentIsDeflateToken, StringComparison.InvariantCultureIgnoreCase) != -1)
                _deflateStream = new DeflateStream(stream, mode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] array, int offset, int count)
        {
            if (_gzipStream != null)
                return _gzipStream.Read(array, offset, count);
            else if (_deflateStream != null)
                return _deflateStream.Read(array, offset, count);

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] array, int offset, int count)
        {
            if (_gzipStream != null)
                _gzipStream.Write(array, offset, count);
            else if (_deflateStream != null)
                _deflateStream.Write(array, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
            if (_gzipStream != null)
                _gzipStream.Flush();
            else if (_deflateStream != null)
                _deflateStream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Utility.SafeDispose( _gzipStream );
            Utility.SafeDispose( _deflateStream );
        }
    }
}
