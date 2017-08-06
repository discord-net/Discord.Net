#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Overwrite
    {
        [ModelProperty("id")]
        public ulong TargetId { get; set; }
        [ModelProperty("type")]
        public PermissionTarget TargetType { get; set; }
        [ModelProperty("deny"), Int53]
        public ulong Deny { get; set; }
        [ModelProperty("allow"), Int53]
        public ulong Allow { get; set; }
    }
}
