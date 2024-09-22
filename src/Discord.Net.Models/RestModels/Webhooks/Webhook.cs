using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class WebhookSourceChannel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}

[DiscriminatedUnionRootType(nameof(Type))]
public class Webhook : IWebhookModel, IModelSource, IModelSourceOf<IUserModel?>
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("user")]
    public Optional<User> Creator { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }
    
    ulong? IWebhookModel.UserId => ~Creator.Map(v => v.Id);

    public IEnumerable<IModel> GetDefinedModels()
    {
        if (Creator) yield return Creator.Value;
    }

    IUserModel? IModelSourceOf<IUserModel?>.Model => ~Creator;
}
