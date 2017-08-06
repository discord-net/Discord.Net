#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class SetLocalVolumeResponse
    {
        [ModelProperty("user_id")]
        public ulong UserId { get; set; }
        [ModelProperty("volume")]
        public int Volume { get; set; }
    }
}
