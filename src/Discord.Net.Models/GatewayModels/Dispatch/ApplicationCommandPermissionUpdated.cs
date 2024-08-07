using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class ApplicationCommandPermissionUpdated : IApplicationCommandPermissionUpdatedPayloadData
{
    [JsonIgnore, JsonExtend]
    public required GuildApplicationCommandPermission GuildApplicationCommandPermission { get; set; }
}
