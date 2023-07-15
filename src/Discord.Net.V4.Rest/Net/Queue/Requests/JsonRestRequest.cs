using Discord.Net.Rest;

namespace Discord.Net.Queue;

public class JsonRestRequest : RestRequest
{
    public string? Json { get; }

    public JsonRestRequest(IRestClient client, string method, string endpoint, string? json, CancellationToken cancellationToken, RequestOptions options)
        : base(client, method, endpoint, cancellationToken, options)
    {
        Json = json;
    }

    public override async Task<RestResponse> SendAsync()
    {
        return await Client.SendAsync(Method, Endpoint, Json, CancellationToken, Options.HeaderOnly, Options.AuditLogReason).ConfigureAwait(false);
    }
}
