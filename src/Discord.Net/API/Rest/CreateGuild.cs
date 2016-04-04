using Discord.Net.JsonConverters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds";
        object IRestRequest.Payload => this;

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("icon"), JsonConverter(typeof(ImageConverter))]
        public Stream Icon { get; set; }
    }
}
