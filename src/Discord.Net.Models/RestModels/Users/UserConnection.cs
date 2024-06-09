using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class UserConnection : IUserConnectionModel, IEntityModelSource
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("revoked")]
    public Optional<bool> IsRevoked { get; set; }

    [JsonPropertyName("integrations")]
    public Optional<Integration[]> Integrations { get; set; }

    [JsonPropertyName("verified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("friend_sync")]
    public bool FriendSync { get; set; }

    [JsonPropertyName("show_activity")]
    public bool ShowActivity { get; set; }

    [JsonPropertyName("two_way_link")]
    public bool IsTwoWayLink { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    bool? IUserConnectionModel.Revoked => IsRevoked;

    IEnumerable<IIntegrationModel>? IUserConnectionModel.Integrations => Integrations | [];
    bool IUserConnectionModel.FriendSyncEnabled => FriendSync;

    public IEnumerable<IEntityModel> GetEntities() => Integrations | [];
}
