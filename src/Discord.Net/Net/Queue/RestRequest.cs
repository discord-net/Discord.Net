using Discord.Net.Rest;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    internal class RestRequest : IQueuedRequest
    {
        public IRestClient Client { get; }
        public string Method { get; }
        public string Endpoint { get; }
        public string Json { get; }
        public bool HeaderOnly { get; }
        public IReadOnlyDictionary<string, object> MultipartParams { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public CancellationToken CancelToken { get; internal set; }

        public bool IsMultipart => MultipartParams != null;

        public RestRequest(IRestClient client, string method, string endpoint, string json, bool headerOnly)
            : this(client, method, endpoint, headerOnly)
        {
            Json = json;
        }

        public RestRequest(IRestClient client, string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly)
            : this(client, method, endpoint, headerOnly)
        {
            MultipartParams = multipartParams;
        }

        private RestRequest(IRestClient client, string method, string endpoint, bool headerOnly)
        {
            Client = client;
            Method = method;
            Endpoint = endpoint;
            Json = null;
            MultipartParams = null;
            HeaderOnly = headerOnly;
            Promise = new TaskCompletionSource<Stream>();
        }

        public async Task<Stream> Send()
        {
            if (IsMultipart)
                return await Client.Send(Method, Endpoint, MultipartParams, HeaderOnly).ConfigureAwait(false);
            else
                return await Client.Send(Method, Endpoint, Json, HeaderOnly).ConfigureAwait(false);
        }
    }
}
