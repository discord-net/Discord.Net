using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateGuildBanParams
    {
        [JsonProperty("delete-message-days")]
        internal Optional<int> _deleteMessageDays;
        public int DeleteMessageDays { set { _deleteMessageDays = value; } }
    }
}
