using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal class InteractionCreated
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("application_id")]
        public ulong ApplicationId { get; set; }

        [JsonProperty("type")]
        public InteractionType Type { get; set; }

        [JsonProperty("data")]
        public Optional<object> Data { get; set; }

        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }

        [JsonProperty("channel_id")]
        public Optional<ulong> ChannelId { get; set; }

        [JsonProperty("member")]
        public Optional<GuildMember> Member { get; set; }

        [JsonProperty("user")]
        public Optional<User> User { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("message")]
        public Optional<Message> Message { get; set; }

    }
}
