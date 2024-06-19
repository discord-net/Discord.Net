using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Ban : IBanModel, IEntityModelSource
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    public IEnumerable<IEntityModel> GetEntities() => [User];
    ulong IBanModel.UserId => User.Id;
}
