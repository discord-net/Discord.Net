#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateGuildBanParams
    {
        [JsonProperty("delete-message-days")]
        public Optional<int> DeleteMessageDays { get; set; }
    }
}
