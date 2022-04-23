using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API
{
    internal class Presence : IPresenceModel
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("status")]
        public UserStatus Status { get; set; }

        [JsonProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nick { get; set; }
        // This property is a Dictionary where each key is the ClientType
        // and the values are the current client status.
        // The client status values are all the same.
        // Example:
        //   "client_status": { "desktop": "dnd", "mobile": "dnd" }
        [JsonProperty("client_status")]
        public Optional<Dictionary<string, string>> ClientStatus { get; set; }
        [JsonProperty("activities")]
        public List<Game> Activities { get; set; }
        [JsonProperty("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; set; }

        ulong IPresenceModel.UserId {
            get => User.Id; set => throw new NotSupportedException();
        }

        ulong? IPresenceModel.GuildId {
            get => GuildId.ToNullable(); set => throw new NotSupportedException();
        }

        UserStatus IPresenceModel.Status {
            get => Status; set => throw new NotSupportedException();
        }

        ClientType[] IPresenceModel.ActiveClients {
            get => ClientStatus.IsSpecified ? ClientStatus.Value.Select(x => (ClientType)Enum.Parse(typeof(ClientType), x.Key, true)).ToArray() : Array.Empty<ClientType>(); set => throw new NotSupportedException();
        }

        IActivityModel[] IPresenceModel.Activities {
            get => Activities.ToArray(); set => throw new NotSupportedException();
        }
        ulong IEntityModel<ulong>.Id {
            get => User.Id; set => throw new NotSupportedException();
        }
    }
}
