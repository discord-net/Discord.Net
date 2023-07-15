using Discord.Net.Rest;

namespace Discord.Net.Queue;

public class RestRequest : IRequest
{
    public IRestClient Client { get; }
    public string Method { get; }
    public string Endpoint { get; }
    public DateTimeOffset? TimeoutAt { get; }
    public TaskCompletionSource<Stream> Promise { get; }
    public RequestOptions Options { get; }
    public CancellationToken CancellationToken { get; }

    public RestRequest(IRestClient client, string method, string endpoint, CancellationToken cancellationToken, RequestOptions options)
    {
        Preconditions.NotNull(options, nameof(options));

        Client = client;
        Method = method;
        Endpoint = endpoint;
        Options = options;
        TimeoutAt = options.Timeout.HasValue
            ? DateTimeOffset.UtcNow.AddMilliseconds(options.Timeout.Value)
            : null;
        Promise = new TaskCompletionSource<Stream>();
        CancellationToken = cancellationToken;
    }

    public virtual async Task<RestResponse> SendAsync()
    {
        return await Client.SendAsync(Method, Endpoint, CancellationToken, Options.HeaderOnly, Options.AuditLogReason, Options.RequestHeaders).ConfigureAwait(false);
    }
}
