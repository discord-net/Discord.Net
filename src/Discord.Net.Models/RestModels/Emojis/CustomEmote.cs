using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[HasPartialVariant]
[DiscriminatedUnionType(nameof(Id), WhenSpecified = true)]
public sealed class CustomEmote : Emote, ICustomEmoteModel, IModelSource, IModelSourceOf<IUserModel?>
{
    [JsonIgnore]
    protected override DiscordEmojiId DiscordEmojiId => new(Name, Id, ~Animated);

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

    ulong[] ICustomEmoteModel.Roles => RoleIds | [];

    bool ICustomEmoteModel.RequireColons => ~RequireColons;
    bool ICustomEmoteModel.IsManaged => ~Managed;
    bool ICustomEmoteModel.IsAnimated => ~Animated;
    bool ICustomEmoteModel.IsAvailable => ~Available;

    ulong? ICustomEmoteModel.UserId => ~User.Map(v => v.Id);

    public IEnumerable<IModel> GetDefinedModels()
    {
        if (User.IsSpecified)
            yield return User.Value;
    }

    IUserModel? IModelSourceOf<IUserModel?>.Model => ~User;
}
