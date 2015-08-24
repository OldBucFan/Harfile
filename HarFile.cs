using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarFile
{
    class HarFile
    {
        public Log log { get; set; }
        public class Log
        {
            public string version { get; set; }
            public Creator creator { get; set; }
            public Page[] pages { get; set; }
            public Entry[] entries { get; set; }
        }

        public class Creator
        {
            public string name { get; set; }
            public string version { get; set; }
        }

        public class Page
        {
            public DateTime startedDateTime { get; set; }
            public string id { get; set; }
            public string title { get; set; }
            public Pagetimings pageTimings { get; set; }
        }

        public class Pagetimings
        {
            public float onContentLoad { get; set; }
            public float onLoad { get; set; }
        }

        public class Entry
        {
            public DateTime startedDateTime { get; set; }
            public float time { get; set; }
            public Request request { get; set; }
            public Response response { get; set; }
            public Cache cache { get; set; }
            public Timings timings { get; set; }
            public string connection { get; set; }
            public string pageref { get; set; }
            }

        public class Request
        {
            public string method { get; set; }
            public string url { get; set; }
            public string httpVersion { get; set; }
            public Header[] headers { get; set; }
            public Querystring[] queryString { get; set; }
            public Cooky[] cookies { get; set; }
            public int headersSize { get; set; }
            public int bodySize { get; set; }
            public PostData postData { get; set; }
        }

        public class PostData
        {
            public string mimeType { get; set; }
            public string text { get; set; }
            public Querystring[] @params { get; set; } // cheating and re-using for parameters since they are just string name value pairs
        }

        public class Header
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Querystring
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Cooky
        {
            public string name { get; set; }
            public string value { get; set; }
            public object expires { get; set; }
            public bool httpOnly { get; set; }
            public bool secure { get; set; }
        }

        public class Response
        {
            public int status { get; set; }
            public string statusText { get; set; }
            public string httpVersion { get; set; }
            public Header[] headers { get; set; }
            public Cooky[] cookies { get; set; }
            public Content content { get; set; }
            public string redirectURL { get; set; }
            public int headersSize { get; set; }
            public int bodySize { get; set; }
            public int _transferSize { get; set; }
        }

        public class Content
        {
            public int size { get; set; }
            public string mimeType { get; set; }
            public int compression { get; set; }
            public string text { get; set; }
        }

        public class Cache
        {
        }

        public class Timings
        {
            public float blocked { get; set; }
            public float dns { get; set; }
            public float connect { get; set; }
            public float send { get; set; }
            public float wait { get; set; }
            public float receive { get; set; }
            public float ssl { get; set; }
        }

        public class Stat
        {
            public string item { get; set; }
            public int count { get; set; }
            public int count200 { get; set; }
            public int count400 { get; set; }
            public int count500 { get; set; }
            public float avgLoad { get; set; }
            public float longestLoad { get; set; }
            public float avgBlocked { get; set; }
            public float longestBlocked { get; set; }
            public float avgDNS { get; set; }
            public float longestDNS { get; set; }
            public float avgConnect { get; set; }
            public float longestConnect { get; set; }
            public float avgSSL { get; set; }
            public float longestSSL { get; set; }
            public float avgSend { get; set; }
            public float longestSend { get; set; }
            public float avgWait { get; set; }
            public float longestWait { get; set; }
            public float avgDownload { get; set; }
            public float longestDownload { get; set; }
            public List<SimpleEntry> entries { get; set; }
        }
        public class SimpleEntry
        {
            public string timestamp { get; set; }
            public string url { get; set; }
            public int responseSize { get; set; }
            public float time { get; set; }

        }
        public class Outlier
        {
            public string timestamp { get; set; }
            public string url { get; set; }
            public int responseSize { get; set; }
            public float time { get; set; }
            public float avgTime { get; set; }
            public int sdCount { get; set; }
            public float sd { get; set; }
            public string type { get; set; } //URL or file type
        }
        public class Error
        {
            public string timestamp { get; set; }
            public float time { get; set; }
            public string item { get; set; }
            public string url { get; set; }
            public Querystring[] queryString { get; set; }
            public int status { get; set; }
            public Header[] responseHeaders { get; set; }
            public string errorText { get; set; }
        }
    }
}
