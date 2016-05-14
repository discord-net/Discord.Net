using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    internal class RestRequest
    {
        public string Method { get; }
        public string Endpoint { get; }
        public string Json { get; }
        public bool HeaderOnly { get; }
        public CancellationToken CancelToken { get; internal set; }
        public IReadOnlyDictionary<string, object> MultipartParams { get; }
        public TaskCompletionSource<Stream> Promise { get; }

        public bool IsMultipart => MultipartParams != null;

        public RestRequest(string method, string endpoint, string json, bool headerOnly)
            : this(method, endpoint, headerOnly)
        {
            Json = json;
        }

        public RestRequest(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly)
            : this(method, endpoint, headerOnly)
        {
            MultipartParams = multipartParams;
        }

        private RestRequest(string method, string endpoint, bool headerOnly)
        {
            Method = method;
            Endpoint = endpoint;
            Json = null;
            MultipartParams = null;
            HeaderOnly = headerOnly;
            Promise = new TaskCompletionSource<Stream>();
        }
    }
}
