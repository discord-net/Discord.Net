using Discord.API.Converters;
using Newtonsoft.Json;
using System.Linq;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ReorderChannelsRequest : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/guilds/{GuildId}/channels";
        object IRestRequest.Payload
        {
            get
            {
                int pos = StartPos;
                return ChannelIds.Select(x => new Channel(x, pos++));
            }
        }
        bool IRestRequest.IsPrivate => false;

        public sealed class Channel
        {
            [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
            public ulong Id;
            [JsonProperty("position")]
            public int Position;

            public Channel(ulong id, int position)
            {
                Id = id;
                Position = position;
            }
        }
        
        public ulong GuildId { get; }

        public ulong[] ChannelIds { get; set; } = new ulong[0];
        public int StartPos { get; set; } = 0;

        public ReorderChannelsRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
