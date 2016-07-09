using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class ModifyCurrentUserParams
    {
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }

        [JsonProperty("avatar")]
        private Optional<Image> _avatar { get; set; }
        [JsonIgnore]
        public Optional<Stream> Avatar
        {
            get { return _avatar.IsSpecified ? _avatar.Value.Stream : null; }
            set { _avatar = value.IsSpecified ? new Image(value.Value) : Optional.Create<Image>(); }
        }
        [JsonIgnore]
        internal Optional<string> AvatarHash
        {
            get { return _avatar.IsSpecified ? _avatar.Value.Hash : null; }
            set { _avatar = value.IsSpecified ? new Image(value.Value) : Optional.Create<Image>(); }
        }
    }
}
