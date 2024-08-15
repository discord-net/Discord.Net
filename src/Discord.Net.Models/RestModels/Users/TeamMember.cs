using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class TeamMember : IModelSource, IModelSourceOf<IUserModel>
{
    [JsonPropertyName("membership_state")]
    public required int MembershipState { get; set; }

    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("team_id")]
    public required ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    public IEnumerable<IModel> GetDefinedModels() => [User];

    IUserModel IModelSourceOf<IUserModel>.Model => User;
}
