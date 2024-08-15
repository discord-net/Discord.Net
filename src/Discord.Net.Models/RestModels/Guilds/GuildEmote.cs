using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[HasPartialVariant]
[DiscriminatedUnionType(nameof(Id), WhenSpecified = true)]
public sealed class GuildEmote : Emote, IGuildEmoteModel, IModelSource, IModelSourceOf<IUserModel?>
{
    [JsonPropertyName("id"), PartialIgnore]
    public required ulong Id { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]> RoleIds { get; set; }

    [JsonPropertyName("user")]
    public Optional<User> User { get; set; }

    [JsonPropertyName("require_colons")]
    public Optional<bool> RequireColons { get; set; }

    [JsonPropertyName("managed")]
    public Optional<bool> Managed { get; set; }

    [JsonPropertyName("animated")]
    public Optional<bool> Animated { get; set; }

    [JsonPropertyName("available")]
    public Optional<bool> Available { get; set; }

    ulong[] IGuildEmoteModel.Roles => RoleIds | [];

    bool IGuildEmoteModel.RequireColons => ~RequireColons;
    bool IGuildEmoteModel.IsManaged => ~Managed;
    bool IGuildEmoteModel.IsAnimated => ~Animated;
    bool IGuildEmoteModel.IsAvailable => ~Available;

    ulong? IGuildEmoteModel.UserId => ~User.Map(v => v.Id);

    public IEnumerable<IModel> GetDefinedModels()
    {
        if (User.IsSpecified)
            yield return User.Value;
    }

    IUserModel? IModelSourceOf<IUserModel?>.Model => ~User;
}
