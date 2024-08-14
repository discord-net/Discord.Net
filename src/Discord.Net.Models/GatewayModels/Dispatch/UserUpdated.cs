using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class UserUpdated : IUserUpdatedPayloadData
{
    [JsonIgnore, JsonExtend]
    public required SelfUser User { get; set; }
}