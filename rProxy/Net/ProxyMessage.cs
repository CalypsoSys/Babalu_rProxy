//#define RAWLOG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Babalu.rProxy
{
    /// <summary>
    /// base class to handle HTTP/HTTPS header/content processing for the proxy
    /// </summary>
    internal abstract class ProxyMessage : IBabaluMessage
    {
        private const string _headerBreakToken = "\r\n\r\n";
        private const string _transferEncdingBreakToken = "\r\n";
        private const char _cToken = 'C';
        private const string _contentStartToken = "Content-";
        private const string _hostToken = "Host:";
        private const string _contentLengthToken = "Content-Length:";
        private const string _contentTypeToken = "Content-Type:";
        private const string _contentEncodingToken = "Content-Encoding:";
        private const string _transferEncodingToken = "Transfer-Encoding:";
        private const string _cacheControlToken = "Cache-Control:";
        private const string _userAgentToken = "User-Agent:";
        private const string _authorizationToken = "Authorization:";
        protected const string _basicAuthorization = "Basic ";
        private static string _basicAuthorizationToken = string.Format( "{0} {1}", _authorizationToken, _basicAuthorization );
        private const string _userQuery = "User=";

        private static string _contentIsTextToken = "text";
        private static string _transferEncodingChunkedToken = "chunked";
        private static string[] _contentIsBinaryTokens = new string[] { "image", "video", "audio" };
        private static string[] _canCacheControlTokens = new string[] { "public", "max-age", "max-stale", "min-fresh", "s-maxage" };

        private IExternalMessageHandler _messageHandler;
        private bool _supportsGZip;
        private byte[] _rawRead;
        private List<byte> _header;
        private List<byte> _content;
        private int _newContentLength = -1;
        private string _contentType = null;
        private string _contentEncoding = null;
        private bool _transferEncodingChunked = false;
        private string _cacheControl = null;
        private int? _cacheAge = null;
        private int _firstHeaderLineEnd;
        private string _firstHeaderLine = null;

#if DEBUG && RAWLOG
        private static volatile object _sync = new object();
#endif

        public abstract int BufferSize { get; }
        protected abstract int SpecialReplace(List<byte> input, int index, List<byte> output);

        /// <summary>
        /// socket input from client or server to process
        /// </summary>
        /// <param name="messageHandler">external message handler</param>
        /// <param name="supportsGZip">are we supporting GZip compression</param>
        /// <param name="input">the client for server HTTP stream</param>
        /// <param name="log">request log structure</param>
        /// <param name="checkPreHttp">is this the request and a pre http 1.1 request</param>
        protected ProxyMessage(IExternalMessageHandler messageHandler, bool supportsGZip, Stream input, LogRequest log, bool checkPreHttp)
        {
            _messageHandler = messageHandler;

            _supportsGZip = supportsGZip;
            _rawRead = new byte[BufferSize];
            _header = new List<byte>(BufferSize);
            _content = new List<byte>(BufferSize);

            int bytes = 0;
            bool headerDone = false;
            bool contentDone = false;
            int contentLength = 0;
            // loop until we have read al the input (no bytes returned, header end character or content length reached)
            do
            {
                bytes = input.Read(_rawRead, 0, BufferSize);

                if (bytes != 0)
                {
                    if (headerDone == false)
                    {
                        // add all bytes read to the header
                        _header.AddRange(_rawRead.Take(bytes));

                        // parse the header information
                        HeaderPositions match = ParseHeader();

                        // if we read the entire HTTP header process it
                        if (match.HeaderEnd != -1)
                        {
                            // find the end of the list line
                            _firstHeaderLineEnd = match.FirstLineEnd;

                            // add any content we may have read into the content buffer based on the header end characters
                            _content.AddRange(_header.Skip(match.HeaderEnd));
                            // trim the header of any content information
                            _header.RemoveRange(match.HeaderEnd, _header.Count - match.HeaderEnd);
                            headerDone = true;

                            // if the header did not contain a content length tag, content is done
                            if (match.ContentLength == -1)
                                contentDone = true;
                            else
                            {
                                // get the "real" content length from the header
                                int pos = match.ContentLength;
                                while (_header[pos] != '\r')
                                {
                                    char c = (char)_header[pos];
                                    if (char.IsNumber(c))
                                        contentLength = ((contentLength * 10) + (c - '0'));
                                    ++pos;
                                }

                                // see if we have alread read in all the content
                                if (contentLength == 0 || _content.Count == contentLength)
                                    contentDone = true;
                            }

                            // get misc. header values
                            _contentType = ParseHeaderValue(match.ContentType);
                            _contentEncoding = ParseHeaderValue(match.ContentEncoding);
                            _cacheControl = ParseHeaderValue(match.CacheControl);

                            string transferEncoding = ParseHeaderValue(match.TransferEncoding);
                            if (transferEncoding != null && transferEncoding.IndexOf(_transferEncodingChunkedToken, StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                _transferEncodingChunked = true;
                                contentDone = ReadChunkedContent();
                            }

                            if (match.Host != -1)
                                Host = ParseHeaderValue(match.Host, ':');

                            // have logging values
                            if (match.UserAgent != -1)
                                UserAgent = log.CsUserAgent = ParseHeaderValue(match.UserAgent);
                            if (match.Authorization != -1)
                                Authorization = ParseHeaderValue(match.Authorization);
                        }
                        else if ( match.FirstLineEnd != -1 && checkPreHttp )
                        {
                            // check pre http 1.1 headers
                            string []parts = Encoding.UTF8.GetString(_header.ToArray()).Split( new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
                            if (parts.Length == 2 && string.Compare(parts[0], "GET", true) == 0)
                            {
                                headerDone = true;
                                contentDone = true;
                            }
                        }
                    }
                    else
                    {
                        // if the header is complete, read the content (if specified in Content-Length)
                        _content.AddRange(_rawRead.Take(bytes));
                        if (_transferEncodingChunked)
                            contentDone = ReadChunkedContent();
                        else
                            contentDone = (_content.Count == contentLength);
                    }
                }
            }
            while (bytes != 0 && (headerDone == false || contentDone == false));

            if (_transferEncodingChunked)
                DeChunkData();

            LogHeaderContent();
        }

        public static void RawLog( ProxyMessage request, ProxyMessage response )
        {
#if DEBUG && RAWLOG
            lock (_sync)
            {
                using (FileStream fs = new FileStream("c:\\mac_raw.data", FileMode.Append, FileAccess.Write))
                {
                    byte[] data = Encoding.ASCII.GetBytes("\r\nStart Conversation\r\n");
                    fs.Write(data, 0, data.Length);

                    request.RawLog(fs);
                    response.RawLog(fs);

                    data = Encoding.ASCII.GetBytes("\r\n\r\nEnd Conversation\r\n");
                    fs.Write(data, 0, data.Length);
                }
            }
#endif
        }

#if DEBUG && RAWLOG
        private void RawLog(FileStream fs)
        {
            byte[] data;

            data = Encoding.ASCII.GetBytes(string.Format("Start Header {0} {1}\r\n", this.GetType().Name, _header.Count));
            fs.Write(data, 0, data.Length);

            data = _header.ToArray();
            fs.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes(string.Format("\r\nEnd Header {0}\r\n", this.GetType().Name));
            fs.Write(data, 0, data.Length);

            if (_content.Count > 0)
            {
                data = Encoding.ASCII.GetBytes(string.Format("Start Content {0} {1}\r\n", this.GetType().Name, _content.Count));
                fs.Write(data, 0, data.Length);

                data = _content.ToArray();
                fs.Write(data, 0, data.Length);
                data = Encoding.ASCII.GetBytes(string.Format("\r\nEnd Content {0}\r\n", this.GetType().Name));
                fs.Write(data, 0, data.Length);

                try
                {
                    byte[] rawConent;
                    if (CompressedStream.IsCompressed(_contentEncoding))
                        rawConent = DeCompressContent();
                    else
                        rawConent = _content.ToArray();

                    WBXMLDocument doc = GetWBXMLContent(rawConent);
                    if (doc != null)
                    {
                        data = Encoding.ASCII.GetBytes(string.Format("Start XBML Content {0} {1}\r\n", this.GetType().Name, _content.Count));
                        fs.Write(data, 0, data.Length);

                        data = Encoding.ASCII.GetBytes(doc.OuterXml);
                        fs.Write(data, 0, data.Length);

                        data = Encoding.ASCII.GetBytes(string.Format("\r\nEnd XBML Content {0}\r\n", this.GetType().Name));
                        fs.Write(data, 0, data.Length);
                    }
                }
                catch
                {
                    //eat it
                }
            }
            else
            {
                data = Encoding.ASCII.GetBytes(string.Format("No Content {0}\r\n", this.GetType().Name));
                fs.Write(data, 0, data.Length);
            }
        }
#endif

        private bool ReadChunkedContent()
        {
            return ProcessChunkedContent(null);
        }

        private void DeChunkData()
        {
            List<byte> content = new List<byte>(_content.Count);
            ProcessChunkedContent(content);
            _content = content;
        }

        private bool ProcessChunkedContent(List<byte> content)
        {
            StringBuilder len = new StringBuilder();
            for (int i = 0;  i < _content.Count; i++)
            {
                if (_content[i] == _transferEncdingBreakToken[0] && Utility.Match(_content, i, _transferEncdingBreakToken))
                {
                    int length;
                    if (int.TryParse(len.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out length))
                    {
                        if (length == 0)
                            return true;
                        else
                        {
                            // 2 because we are on \r\n are on each side and the loop above will also add 1 so minus 1
                            int pos = i + length + (_transferEncdingBreakToken.Length * 2) -1;
                            if (content != null)
                                content.AddRange(_content.Skip(i + _transferEncdingBreakToken.Length).Take(length));

                            if (pos >= _content.Count)
                                return false;
                            else
                                i = pos;
                        }
                    }
                    len.Length = 0;
                }
                else
                    len.Append((char)_content[i]);
            }

            return false;
        }

        /// <summary>
        /// log the header and content 
        /// </summary>
        private void LogHeaderContent()
        {
            // if there is header inforamtion, log it
            if (_header.Count > 0)
            {
                try
                {
                    // log header information
                    string headerDebugString = Encoding.UTF8.GetString(_header.ToArray());

                    // if logging debug information log the raw header and raw/processed content
                    if (BabaluConfigurationFactory.Instance.LogDebug)
                    {
                        string contentDebugString = string.Empty;
                        if (_content.Count > 0)
                        {
                            byte[] rawContent;
                            contentDebugString = _messageHandler.GetContentForLogging(this, out rawContent);
                            if (contentDebugString == null)
                            {
                                if (rawContent == null)
                                {
                                    if (_supportsGZip && CompressedStream.IsCompressed(_contentEncoding))
                                        rawContent = CompressedStream.DeCompressContent(_content.ToArray(), _contentEncoding, _rawRead, BufferSize);
                                    else
                                        rawContent = _content.ToArray();
                                }
                                contentDebugString = Encoding.ASCII.GetString(rawContent);
                            }
                        }
                        LogFactory.LogDebug("Original\r\n{0}{1}", headerDebugString, contentDebugString);
                    }
                }
                catch (Exception excp)
                {
                    LogFactory.LogException(excp, "LogHeaderContent exception");
                }
            }
        }

        /// <summary>
        /// return the first line of the HTPP header
        /// </summary>
        protected string FirstHeaderLine
        {
            get
            {
                if (_firstHeaderLine == null)
                    _firstHeaderLine = Encoding.UTF8.GetString(_header.Take(_firstHeaderLineEnd).ToArray());

                return _firstHeaderLine;
            }
        }

        /// <summary>
        /// get the header "Host" 
        /// </summary>
        public string Host
        {
            get;
            private set;
        }

        /// <summary>
        /// get the header "Authorization" 
        /// </summary>
        protected string Authorization
        {
            get; private set;
        }

        /// <summary>
        /// get the header "User-Agent" 
        /// </summary>
        public string UserAgent
        {
            get; private set;
        }

        /// <summary>
        /// process the header "Cache-Control" information, -1 is forever, 0 is never
        /// </summary>
        protected int CacheAge
        {
            get
            {
                if (_cacheAge.HasValue == false)
                {
                    _cacheAge = 0;
                    if (_cacheControl != null)
                    {
                        // there is a "Cache-Control" header token
                        LogFactory.LogDebug("Cache type {0}", _cacheControl);

                        // loop through allowable cachable tokens
                        foreach (string control in _canCacheControlTokens)
                        {
                            // see if the control token is in the allowable list of cache items
                            int index = _cacheControl.IndexOf(control, StringComparison.InvariantCultureIgnoreCase);
                            if (index != -1)
                            {
                                // read in the max-age for this cache item 
                                string token = _cacheControl.Substring(index);
                                int cacheAge = -1;
                                foreach (char c in token)
                                {
                                    if (char.IsNumber(c))
                                    {
                                        if (cacheAge == -1)
                                            cacheAge = 0;
                                        cacheAge = ((cacheAge * 10) + (c - '0'));
                                    }
                                }

                                _cacheAge = cacheAge;
                            }
                        }
                    }
                }

                return _cacheAge.Value;
            }
        }

        /// <summary>
        /// have we read any data from this socket stream
        /// </summary>
        public bool HasData
        {
            get { return _header.Count > 0; }
        }

        /// <summary>
        /// tranform a request/response from one server to a request/response to another server
        /// </summary>
        /// <param name="match"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public byte[] Tranform(string match, string replace)
        {
            // allocate total amount (header+content) we are going to write out
            List<byte> output = new List<byte>(_header.Count + _content.Count);

            // see if the content type is binary, if so no rewrite is necessary
            byte[] content;
            if (IsContentBinary == false)
            {
                // if no content 
                if (_content.Count == 0)
                    content = _content.ToArray();
                else if (_supportsGZip && CompressedStream.IsCompressed(_contentEncoding))
                    content = ProcessCompressedContent(match, replace);
                else
                {
                    content = Replace(_content, match, replace).ToArray();
                    content = _messageHandler.ProcessResponseContent(this, content);
                }
            }
            else
            {
                // set flag to leave content length in header alone
                content = _content.ToArray();
            }

            // set flag to fix content length in header
            _newContentLength = -1;
            if (_content.Count != 0)
            {
                if (_transferEncodingChunked == false && content.Length != _content.Count)
                    _newContentLength = content.Length;
            }

            output.AddRange(Replace(_header, match, replace));
            if (_transferEncodingChunked)
                output.AddRange(PreChunk(content.Length));
            output.AddRange(content);
            if (_transferEncodingChunked)
                output.AddRange(PostChunk());

            return output.ToArray();
        }

        private byte[] PreChunk(int contentLength)
        {
            List<byte> preChunk = new List<byte>();
            foreach (char c in string.Format( "{0}\r\n", contentLength.ToString("X")))
                preChunk.Add((byte)c);
            return preChunk.ToArray();
        }

        private byte[] PostChunk()
        {
            List<byte> postChunk = new List<byte>();
            foreach (char c in "\r\n0\r\n\r\n")
                postChunk.Add((byte)c);
            return postChunk.ToArray();
        }


        /// <summary>
        /// look at the content types to see if it is binary data, less processing
        /// </summary>
        /// <returns></returns>
        private bool IsContentBinary
        {
            get
            {
                // see if the content type is binary, if so no rewrite is necessary
                if (_contentType != null)
                {
                    // look for expected binary content types
                    if (_contentType.IndexOf(_contentIsTextToken, StringComparison.InvariantCultureIgnoreCase) == -1)
                    {
                        foreach (string token in _contentIsBinaryTokens)
                        {
                            if (_contentType.IndexOf(token, StringComparison.InvariantCultureIgnoreCase) != -1)
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        private byte[] ProcessCompressedContent(string match, string replace)
        {
            try
            {
                // deal with gzipped data
                byte[] rawContent = CompressedStream.DeCompressContent(_content.ToArray(), _contentEncoding, _rawRead, BufferSize);

                // do Tranform on decompressed data
                rawContent = Replace(new List<byte>(rawContent), match, replace).ToArray();
                rawContent = _messageHandler.ProcessResponseContent(this, rawContent);

                // we have fixed up and decompressed the gzipped data, now gzip (compress) it back up
                using (MemoryStream rewriteStream = new MemoryStream())
                {
                    using (CompressedStream compressStream = new CompressedStream(_contentEncoding, rewriteStream, CompressionMode.Compress))
                    {
                        compressStream.Write(rawContent, 0, rawContent.Length);
                        compressStream.Flush();
                        rewriteStream.Flush();
                    }
                    return rewriteStream.ToArray();
                }
            }
            catch (Exception excp)
            {
                LogFactory.LogException(excp, "compression");
                // try to return just the raw data
                return _content.ToArray();
            }
        }

        /// <summary>
        /// replace value in a raw data
        /// </summary>
        /// <param name="input">raw data</param>
        /// <param name="match">search string</param>
        /// <param name="replace">replace string</param>
        /// <returns></returns>
        private List<byte> Replace(List<byte> input, string match, string replace)
        {
            List<byte> output = new List<byte>(input.Count);
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == match[0] && Utility.Match(input, i, match))
                {
                    foreach (char c in replace)
                        output.Add((byte)c);
                    i += match.Length;
                }
                else if (_messageHandler.ReplaceOverride(this, output, input, ref i))
                {
                }
                else if (_newContentLength != -1 && input[i] == _contentLengthToken[0] && Utility.Match(input, i, _contentLengthToken))
                {
                    // fix up content length in the header
                    foreach (char c in _contentLengthToken)
                        output.Add((byte)c);
                    output.Add((byte)' ');
                    foreach (char c in _newContentLength.ToString())
                        output.Add((byte)c);
                    while (input[i] != '\r')
                    {
                        ++i;
                    }
                }
                else if (_supportsGZip == false)
                    i = SpecialReplace(input, i, output);

                output.Add(input[i]);
            }

            _messageHandler.ReplaceOutput(this, output);

            return output;
        }

        protected string ProcessUserNameAndPassword(string unPw, bool stripPw, string defaultValue)
        {
            string fullUnPw = Utility.DecodeFrom64(unPw);
            fullUnPw = _messageHandler.ProcessUserNameAndPassword(fullUnPw);

            if ( stripPw )
            {
                int index = fullUnPw.IndexOf(':');
                if (index != -1)
                    fullUnPw = fullUnPw.Substring(0, index);
                else
                    fullUnPw = defaultValue;
            }

            return fullUnPw;
        }

        /// <summary>
        /// look for specific header values and store their index in the header data
        /// </summary>
        /// <returns></returns>
        private HeaderPositions ParseHeader()
        {
            HeaderPositions positions = new HeaderPositions();
            for (int i = 0; positions.HeaderEnd == -1 && i < _header.Count; i++)
            {
                if (positions.FirstLineEnd == -1 && _header[i] == '\r')
                    positions.FirstLineEnd = i;

                if (_header[i] == _headerBreakToken[0] && Utility.Match(_header, i, _headerBreakToken))
                    positions.HeaderEnd = i + _headerBreakToken.Length;
                else if (_header[i] == _cToken)
                {
                    if (Utility.Match(_header, i, _contentStartToken))
                    {
                        if (Utility.Match(_header, i, _contentLengthToken))
                            positions.ContentLength = i + _contentLengthToken.Length;
                        else if (Utility.Match(_header, i, _contentTypeToken))
                            positions.ContentType = i + _contentTypeToken.Length;
                        else if (Utility.Match(_header, i, _contentEncodingToken))
                            positions.ContentEncoding = i + _contentEncodingToken.Length;
                    }
                    else if (Utility.Match(_header, i, _cacheControlToken))
                        positions.CacheControl = i + _cacheControlToken.Length;
                }
                else if (_header[i] == _transferEncodingToken[0] && Utility.Match(_header, i, _transferEncodingToken))
                    positions.TransferEncoding = i + _transferEncodingToken.Length;
                else if (_header[i] == _userAgentToken[0] && Utility.Match(_header, i, _userAgentToken))
                    positions.UserAgent = i + _userAgentToken.Length;
                else if (_header[i] == _hostToken[0] && Utility.Match(_header, i, _hostToken))
                    positions.Host = i + _hostToken.Length;
                else if (_header[i] == _authorizationToken[0] && Utility.Match(_header, i, _authorizationToken))
                    positions.Authorization = i + _authorizationToken.Length;

            }

            return positions;
        }


        /// <summary>
        /// read a header value starting at the defined index until we reach the first new-line character
        /// </summary>
        /// <param name="index"></param>
        /// <param name="alternate"></param>
        /// <returns></returns>
        private string ParseHeaderValue(int index, char alternate = '\0')
        {
            if (index != -1)
            {
                StringBuilder value = new StringBuilder();
                while (_header[index] != '\r' && (alternate == '\0' || _header[index] != alternate))
                {
                    value.Append((char)_header[index]);
                    ++index;
                }
                return value.ToString().Trim();
            }

            return null;
        }

        bool IBabaluMessage.SupportsGzip
        {
            get { return _supportsGZip; }
        }


        string IBabaluMessage.ContentEncoding
        {
            get { return _contentEncoding; }
        }


        string IBabaluMessage.ContentType
        {
            get { return _contentType; }
        }

        byte[] IBabaluMessage.Content
        {
            get { return _content.ToArray(); }
        }


        byte[] IBabaluMessage.RawRead
        {
            get { return _rawRead; }
        }

        string IBabaluMessage.BasicAuthorizationToken
        {
            get { return _basicAuthorizationToken; }
        }

        string IBabaluMessage.UserQuery
        {
            get { return _userQuery; }
        }
    }
}
