using Discord.API.Converters;
using Newtonsoft.Json;
using System.Linq;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ReorderChannelsRequest : IRestRequest
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/channels";
        object IRestRequest.Payload
        {
            get
            {
                int pos = StartPos;
                return ChannelIds.Select(x => new Channel(x, pos++));
            }
        }

        public class Channel
        {
            [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
            public ulong Id { get; set; }
            [JsonProperty("position")]
            public int Position { get; set; }

            public Channel(ulong id, int position)
            {
                Id = id;
                Position = position;
            }
        }
        
        public ulong GuildId { get; set; }

        public ulong[] ChannelIds { get; set; } = new ulong[0];
        public int StartPos { get; set; } = 0;

        public ReorderChannelsRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
