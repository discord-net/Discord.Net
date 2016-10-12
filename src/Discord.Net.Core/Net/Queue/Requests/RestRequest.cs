using Discord.Net.Rest;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class RestRequest : IRequest
    {
        public IRestClient Client { get; }
        public string Method { get; }
        public string Endpoint { get; }
        public string BucketId { get; }
        public DateTimeOffset? TimeoutAt { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public RequestOptions Options { get; }
        public CancellationToken CancelToken { get; internal set; }

        public RestRequest(IRestClient client, string method, string endpoint, string bucketId, RequestOptions options)
        {
            Preconditions.NotNull(options, nameof(options));

            Client = client;
            Method = method;
            Endpoint = endpoint;
            BucketId = bucketId;
            Options = options;
            TimeoutAt = options.Timeout.HasValue ? DateTimeOffset.UtcNow.AddMilliseconds(options.Timeout.Value) : (DateTimeOffset?)null;
            Promise = new TaskCompletionSource<Stream>();
        }

        public virtual async Task<RestResponse> SendAsync()
        {
            return await Client.SendAsync(Method, Endpoint, Options).ConfigureAwait(false);
        }
    }
}
