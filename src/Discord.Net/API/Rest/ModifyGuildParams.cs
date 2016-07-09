using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class ModifyGuildParams
    {
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }

        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("region")]
        public Optional<IVoiceRegion> Region { get; set; }
        [JsonProperty("verification_level")]
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        [JsonProperty("default_message_notifications")]
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        [JsonProperty("afk_timeout")]
        public Optional<int> AFKTimeout { get; set; }

        [JsonProperty("icon")]
        private Optional<Image> _icon { get; set; }
        [JsonIgnore]
        public Optional<Stream> Icon
        {
            get { return _icon.IsSpecified ? _icon.Value.Stream : null; }
            set { _icon = value.IsSpecified ? new Image(value.Value) : Optional.Create<Image>(); }
        }
        [JsonIgnore]
        internal Optional<string> IconHash
        {
            get { return _icon.IsSpecified ? _icon.Value.Hash : null; }
            set { _icon = value.IsSpecified ? new Image(value.Value) : Optional.Create<Image>(); }
        }

        [JsonProperty("splash")]
        private Optional<Image> _splash { get; set; }
        [JsonIgnore]
        public Optional<Stream> Splash
        {
            get { return _splash.IsSpecified ? _splash.Value.Stream : null; }
            set { _splash = value.IsSpecified ? new Image(value.Value) : Optional.Create<Image>(); }
        }
        [JsonIgnore]
        internal Optional<string> SplashHash
        {
            get { return _splash.IsSpecified ? _splash.Value.Hash : null; }
            set { _splash = value.IsSpecified ? new Image(value.Value) : Optional.Create<Image>(); }
        }

        [JsonProperty("afk_channel_id")]
        public Optional<ulong?> AFKChannelId { get; set; }
        [JsonIgnore]
        public Optional<IVoiceChannel> AFKChannel { set { OwnerId = value.IsSpecified ? value.Value.Id : Optional.Create<ulong>(); } }

        [JsonProperty("owner_id")]
        public Optional<ulong> OwnerId { get; set; }
        [JsonIgnore]
        public Optional<IGuildUser> Owner { set { OwnerId = value.IsSpecified ? value.Value.Id : Optional.Create<ulong>(); } }
    }
}
