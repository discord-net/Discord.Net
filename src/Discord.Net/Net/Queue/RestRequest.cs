using Discord.Net.Rest;
using System;
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
        public int? TimeoutTick { get; }
        public IReadOnlyDictionary<string, object> MultipartParams { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public CancellationToken CancelToken { get; set; }

        public bool IsMultipart => MultipartParams != null;

        public RestRequest(IRestClient client, string method, string endpoint, string json, bool headerOnly, RequestOptions options)
            : this(client, method, endpoint, headerOnly, options)
        {
            Json = json;
        }

        public RestRequest(IRestClient client, string method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, bool headerOnly, RequestOptions options)
            : this(client, method, endpoint, headerOnly, options)
        {
            MultipartParams = multipartParams;
        }

        private RestRequest(IRestClient client, string method, string endpoint, bool headerOnly, RequestOptions options)
        {
            if (options == null)
                options = RequestOptions.Default;

            Client = client;
            Method = method;
            Endpoint = endpoint;
            Json = null;
            MultipartParams = null;
            HeaderOnly = headerOnly;
            TimeoutTick = options.Timeout.HasValue ? (int?)unchecked(Environment.TickCount + options.Timeout.Value) : null;
            Promise = new TaskCompletionSource<Stream>();
        }

        public async Task<Stream> SendAsync()
        {
            if (IsMultipart)
                return await Client.SendAsync(Method, Endpoint, MultipartParams, HeaderOnly).ConfigureAwait(false);
            else if (Json != null)
                return await Client.SendAsync(Method, Endpoint, Json, HeaderOnly).ConfigureAwait(false);
            else
                return await Client.SendAsync(Method, Endpoint, HeaderOnly).ConfigureAwait(false);
        }
    }
}
