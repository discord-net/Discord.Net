#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyCurrentUserNickParams
    {
        [ModelProperty("nick")]
        public string Nickname { get; }

        public ModifyCurrentUserNickParams(string nickname)
        {
            Nickname = nickname;
        }
    }
}
