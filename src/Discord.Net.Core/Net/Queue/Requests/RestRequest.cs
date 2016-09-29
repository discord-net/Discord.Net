using Discord.Net.Rest;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class RestRequest : IQueuedRequest
    {
        public IRestClient Client { get; }
        public string Method { get; }
        public string Endpoint { get; }
        public int? TimeoutTick { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public RequestOptions Options { get; }
        public CancellationToken CancelToken { get; internal set; }

        public RestRequest(IRestClient client, string method, string endpoint, RequestOptions options)
        {
            Preconditions.NotNull(options, nameof(options));

            Client = client;
            Method = method;
            Endpoint = endpoint;
            Options = options;
            TimeoutTick = options.Timeout.HasValue ? (int?)unchecked(Environment.TickCount + options.Timeout.Value) : null;
            Promise = new TaskCompletionSource<Stream>();
        }

        public virtual async Task<Stream> SendAsync()
        {
            return await Client.SendAsync(Method, Endpoint, Options).ConfigureAwait(false);
        }
    }
}
