using Discord.Net.Rest;
using System;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class JsonRestRequest : RestRequest
    {
        public ReadOnlyBuffer<byte> Payload { get; }

        public JsonRestRequest(IRestClient client, string method, string endpoint, ReadOnlyBuffer<byte> payload, RequestOptions options)
            : base(client, method, endpoint, options)
        {
            Payload = payload;
        }

        public override async Task<RestResponse> SendAsync()
        {
            return await Client.SendAsync(Method, Endpoint, Payload, Options.CancelToken, Options.HeaderOnly, Options.AuditLogReason).ConfigureAwait(false);
        }
    }
}
