using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildParams
    {
        [JsonProperty("username")]
        internal Optional<string> _username { get; set; }
        public string Username { set { _username = value; } }

        [JsonProperty("name")]
        internal Optional<string> _name { get; set; }
        public string Name { set { _name = value; } }

        [JsonProperty("region")]
        internal Optional<IVoiceRegion> _region { get; set; }
        public IVoiceRegion Region { set { _region = Optional.Create(value); } }

        [JsonProperty("verification_level")]
        internal Optional<VerificationLevel> _verificationLevel { get; set; }
        public VerificationLevel VerificationLevel { set { _verificationLevel = value; } }

        [JsonProperty("default_message_notifications")]
        internal Optional<DefaultMessageNotifications> _defaultMessageNotifications { get; set; }
        public DefaultMessageNotifications DefaultMessageNotifications { set { _defaultMessageNotifications = value; } }

        [JsonProperty("afk_timeout")]
        internal Optional<int> _afkTimeout { get; set; }
        public int AFKTimeout { set { _afkTimeout = value; } }

        [JsonProperty("icon")]
        internal Optional<Image?> _icon { get; set; }
        public Stream Icon { set { _icon = value != null ? new Image(value) : (Image?)null; } }

        [JsonProperty("splash")]
        internal Optional<Image?> _splash { get; set; }
        public Stream Splash { set { _splash = value != null ? new Image(value) : (Image?)null; } }

        [JsonProperty("afk_channel_id")]
        internal Optional<ulong?> _afkChannelId { get; set; }
        public ulong? AFKChannelId { set { _afkChannelId = value; } }
        public IVoiceChannel AFKChannel { set { _afkChannelId = value?.Id; } }

        [JsonProperty("owner_id")]
        internal Optional<ulong> _ownerId { get; set; }
        public ulong OwnerId { set { _ownerId = value; } }
        public IGuildUser Owner { set { _ownerId = value.Id; } }
    }
}
