using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationCommandInteractionData : InteractionData, IApplicationCommandInteractionDataModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("resolved")]
    public Optional<InteractionDataResolved> Resolved { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandInteractionInteractionDataOption[]> Options { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public Optional<ulong> TargetId { get; set; }

    IResolvedDataModel? IApplicationCommandInteractionDataModel.Resolved => ~Resolved;
    IEnumerable<IApplicationCommandInteractionOptionModel>? IApplicationCommandInteractionDataModel.Options => ~Options;
    ulong? IApplicationCommandInteractionDataModel.GuildId => GuildId.ToNullable();
    ulong? IApplicationCommandInteractionDataModel.TargetId => TargetId.ToNullable();
}
