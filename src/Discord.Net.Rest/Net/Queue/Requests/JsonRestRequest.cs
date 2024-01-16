using Discord.Net.Rest;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class JsonRestRequest : RestRequest
    {
        public string Json { get; }

        public JsonRestRequest(IRestClient client, string method, string endpoint, string json, RequestOptions options)
            : base(client, method, endpoint, options)
        {
            Json = json;
        }

        public override Task<RestResponse> SendAsync()
            => Client.SendAsync(Method, Endpoint, Json, Options.CancelToken, Options.HeaderOnly, Options.AuditLogReason);
    }
}
