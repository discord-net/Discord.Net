using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class IntegrationApplication : IApplicationModel, IModelSource, IModelSourceOf<IUserModel?>
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("bot")]
    public Optional<User> Bot { get; set; }

    ulong? IApplicationModel.BotId => ~Bot.Map(v => v.Id);

    public IEnumerable<IEntityModel> GetDefinedModels()
    {
        if (Bot.IsSpecified)
            yield return Bot.Value;
    }

    IUserModel? IModelSourceOf<IUserModel?>.Model => ~Bot;
}
