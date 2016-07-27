using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildParams
    {
        [JsonProperty("username")]
        internal Optional<string> _username;
        public string Username { set { _username = value; } }

        [JsonProperty("name")]
        internal Optional<string> _name;
        public string Name { set { _name = value; } }

        [JsonProperty("region")]
        internal Optional<IVoiceRegion> _region;
        public IVoiceRegion Region { set { _region = Optional.Create(value); } }

        [JsonProperty("verification_level")]
        internal Optional<VerificationLevel> _verificationLevel;
        public VerificationLevel VerificationLevel { set { _verificationLevel = value; } }

        [JsonProperty("default_message_notifications")]
        internal Optional<DefaultMessageNotifications> _defaultMessageNotifications;
        public DefaultMessageNotifications DefaultMessageNotifications { set { _defaultMessageNotifications = value; } }

        [JsonProperty("afk_timeout")]
        internal Optional<int> _afkTimeout;
        public int AFKTimeout { set { _afkTimeout = value; } }

        [JsonProperty("icon")]
        internal Optional<Image?> _icon;
        public Stream Icon { set { _icon = value != null ? new Image(value) : (Image?)null; } }

        [JsonProperty("splash")]
        internal Optional<Image?> _splash;
        public Stream Splash { set { _splash = value != null ? new Image(value) : (Image?)null; } }

        [JsonProperty("afk_channel_id")]
        internal Optional<ulong?> _afkChannelId;
        public ulong? AFKChannelId { set { _afkChannelId = value; } }
        public IVoiceChannel AFKChannel { set { _afkChannelId = value?.Id; } }

        [JsonProperty("owner_id")]
        internal Optional<ulong> _ownerId;
        public ulong OwnerId { set { _ownerId = value; } }
        public IGuildUser Owner { set { _ownerId = value.Id; } }
    }
}
