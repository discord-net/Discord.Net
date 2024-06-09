using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Interaction : IEntityModelSource
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("data")]
    public Optional<InteractionData> Data { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("channel")]
    public Optional<Channel> Channel { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("member")]
    public Optional<GuildMember> Member { get; set; }

    [JsonPropertyName("user")]
    public Optional<User> User { get; set; }

    [JsonPropertyName("token")]
    public required string Token { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("message")]
    public Optional<Message> Message { get; set; }

    [JsonPropertyName("app_permissions")]
    public Optional<ulong> AppPermission { get; set; }

    [JsonPropertyName("locale")]
    public Optional<string> UserLocale { get; set; }

    [JsonPropertyName("guild_locale")]
    public Optional<string> GuildLocale { get; set; }

    public IEnumerable<IEntityModel> GetEntities()
    {
        if (Channel.IsSpecified)
            yield return Channel.Value;

        if (Member.IsSpecified)
            yield return Member.Value;

        if (User.IsSpecified)
            yield return User.Value;

        if (Message.IsSpecified)
            yield return Message.Value;
    }
}
