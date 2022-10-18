using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyCurrentUserNickParams
    {
        [JsonPropertyName("nick")]
        public string Nickname { get; }

        public ModifyCurrentUserNickParams(string nickname)
        {
            Nickname = nickname;
        }
    }
}
