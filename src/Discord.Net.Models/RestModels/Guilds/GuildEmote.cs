using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildEmote : IEmote, IGuildEmoteModel
{
    [JsonPropertyName("id")]
    public required ulong Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

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

    bool IGuildEmoteModel.RequireColons => RequireColons;
    bool IGuildEmoteModel.IsManaged => Managed;
    bool IGuildEmoteModel.IsAnimated => Animated;
    bool IGuildEmoteModel.IsAvailable => Available;

    ulong IGuildEmoteModel.UserId => User.Map(v => v.Id);
}
