using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildScheduledEventUser :
    IGuildScheduledEventUserModel,
    IModelSource,
    IModelSourceOf<IUserModel>,
    IModelSourceOf<IMemberModel?>
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong GuildScheduledEventId { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("member")]
    public Optional<GuildMember> Member { get; set; }

    public IEnumerable<IModel> GetDefinedModels()
    {
        yield return User;

        if (Member)
            yield return Member.Value;

    }

    IMemberModel? IModelSourceOf<IMemberModel?>.Model => ~Member;
    IUserModel IModelSourceOf<IUserModel>.Model => User;
    ulong IGuildScheduledEventUserModel.UserId => User.Id;
}
