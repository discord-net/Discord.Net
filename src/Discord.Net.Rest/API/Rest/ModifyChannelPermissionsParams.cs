#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyChannelPermissionsParams
    {
        [ModelProperty("type")]
        public string Type { get; }
        [ModelProperty("allow")]
        public ulong Allow { get; }
        [ModelProperty("deny")]
        public ulong Deny { get; }

        public ModifyChannelPermissionsParams(string type, ulong allow, ulong deny)
        {
            Type = type;
            Allow = allow;
            Deny = deny;
        }
    }
}
