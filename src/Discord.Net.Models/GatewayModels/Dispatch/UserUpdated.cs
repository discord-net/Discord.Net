using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class UserUpdated : IUserUpdatedPayloadData
{
    [JsonIgnore, JsonExtend] public SelfUser User { get; set; } = null!;
}
