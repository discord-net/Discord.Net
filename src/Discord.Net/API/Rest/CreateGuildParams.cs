using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class CreateGuildParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("icon")]
        private Optional<Image> _icon { get; set; }
        [JsonIgnore]
        public Optional<Stream> Icon
        {
            get { return _icon.IsSpecified ? _icon.Value.Stream : null; }
            set { _icon = value.IsSpecified ? new Image(value.Value) : Optional.Create<Image>(); }
        }
    }
}
