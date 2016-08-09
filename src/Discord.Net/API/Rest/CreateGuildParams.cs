#pragma warning disable CS1591
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateGuildParams
    {
        [JsonProperty("name")]
        public string Name { internal get; set; }

        [JsonProperty("region")]
        public string Region { internal get; set; }

        [JsonProperty("icon")]
        internal Optional<Image?> _icon { get; set; }
        public Stream Icon { set { _icon = value != null ? new Image(value) : (Image?)null; } }
    }
}
