#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyCurrentUserNickParams
    {
        public ModifyCurrentUserNickParams(string nickname)
        {
            Nickname = nickname;
        }

        [JsonProperty("nick")] public string Nickname { get; }
    }
}
