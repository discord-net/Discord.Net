using Newtonsoft.Json;

namespace Discord.API.Gateway;

internal class SoundboardSoundDeletedEvent
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("sound_id")]
    public ulong SoundId { get; set; }
}
