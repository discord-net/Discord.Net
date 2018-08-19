using System.Threading.Tasks;
using Discord.Net.Rest;

namespace Discord.Net.Queue
{
    public class JsonRestRequest : RestRequest
    {
        public JsonRestRequest(IRestClient client, string method, string endpoint, string json, RequestOptions options)
            : base(client, method, endpoint, options)
        {
            Json = json;
        }

        public string Json { get; }

        public override async Task<RestResponse> SendAsync() => await Client
            .SendAsync(Method, Endpoint, Json, Options.CancelToken, Options.HeaderOnly, Options.AuditLogReason)
            .ConfigureAwait(false);
    }
}
