using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Net.Rest;

namespace Discord.Net.Queue
{
    public class RestRequest : IRequest
    {
        public RestRequest(IRestClient client, string method, string endpoint, RequestOptions options)
        {
            Preconditions.NotNull(options, nameof(options));

            Client = client;
            Method = method;
            Endpoint = endpoint;
            Options = options;
            TimeoutAt = options.Timeout.HasValue
                ? DateTimeOffset.UtcNow.AddMilliseconds(options.Timeout.Value)
                : (DateTimeOffset?)null;
            Promise = new TaskCompletionSource<Stream>();
        }

        public IRestClient Client { get; }
        public string Method { get; }
        public string Endpoint { get; }
        public TaskCompletionSource<Stream> Promise { get; }
        public DateTimeOffset? TimeoutAt { get; }
        public RequestOptions Options { get; }

        public virtual async Task<RestResponse> SendAsync() => await Client
            .SendAsync(Method, Endpoint, Options.CancelToken, Options.HeaderOnly, Options.AuditLogReason)
            .ConfigureAwait(false);
    }
}
