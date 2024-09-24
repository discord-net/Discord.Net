using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(GuildId), WhenSpecified = true)]
public sealed class GuildSoundboardSound : SoundboardSound, IGuildSoundboardSoundModel, IModelSourceOf<IUserModel?>
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
    
    [JsonPropertyName("user")]
    public Optional<User> User { get; set; }

    ulong? IGuildSoundboardSoundModel.UserId => User.Map(v => v.Id).ToNullable();

    IUserModel? IModelSourceOf<IUserModel?>.Model => ~User;
}