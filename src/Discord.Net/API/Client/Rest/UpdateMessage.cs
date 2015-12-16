using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UpdateMessageRequest : IRestRequest<Message>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/messages/{MessageId}";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        public ulong ChannelId { get; }
        public ulong MessageId { get; }

        [JsonProperty("content")]
        public string Content { get; set; } = "";
        [JsonProperty("mentions"), JsonConverter(typeof(LongStringArrayConverter))]
        public ulong[] MentionedUserIds { get; set; } = new ulong[0];

        public UpdateMessageRequest(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}
