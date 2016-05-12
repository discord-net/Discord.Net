using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    internal struct RestRequest
    {
        public string Method { get; }
        public string Endpoint { get; }
        public string Json { get; }
        public IReadOnlyDictionary<string, object> MultipartParams { get; }
        public TaskCompletionSource<Stream> Promise { get; }

        public bool IsMultipart => MultipartParams != null;

        public RestRequest(string method, string endpoint, string json)
            : this(method, endpoint)
        {
            Json = json;
        }

        public RestRequest(string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams)
            : this(method, endpoint)
        {
            MultipartParams = multipartParams;
        }

        private RestRequest(string method, string endpoint)
        {
            Method = method;
            Endpoint = endpoint;
            Json = null;
            MultipartParams = null;
            Promise = new TaskCompletionSource<Stream>();
        }
    }
}
