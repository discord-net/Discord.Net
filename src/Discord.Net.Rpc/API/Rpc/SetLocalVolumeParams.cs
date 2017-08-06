#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class SetLocalVolumeParams
    {
        [ModelProperty("volume")]
        public int Volume { get; set; }
    }
}
