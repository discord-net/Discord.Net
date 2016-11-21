using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BlockUserParams
    {
        [JsonProperty("type")]
        public RelationshipType Type { get; } = RelationshipType.Blocked;
    }
}
