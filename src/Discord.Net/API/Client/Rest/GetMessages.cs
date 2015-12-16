using Newtonsoft.Json;
using System.Text;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetMessagesRequest : IRestRequest<Message[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint
        {
            get
            {
                StringBuilder query = new StringBuilder();
                this.AddQueryParam(query, "limit", Limit.ToString());
                if (RelativeDir != null)
                    this.AddQueryParam(query, RelativeDir, RelativeId.Value.ToString());
                return $"{DiscordConfig.ClientAPIUrl}/channels/{ChannelId}/messages{query}";
            }
        }
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; }

        public int Limit { get; set; } = 100;
        public string RelativeDir { get; set; } = null;
        public ulong? RelativeId { get; set; } = 0;

        public GetMessagesRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
