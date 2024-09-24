using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class TeamMember : IModelSource, IModelSourceOf<IPartialUserModel>, ITeamMember
{
    [JsonPropertyName("membership_state")]
    public required int MembershipState { get; set; }

    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("team_id")]
    public required ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public required PartialUser User { get; set; }

    public IEnumerable<IModel> GetDefinedModels() => [User];

    IPartialUserModel IModelSourceOf<IPartialUserModel>.Model => User;

    ulong IEntityModel<ulong>.Id => User.Id;
}
