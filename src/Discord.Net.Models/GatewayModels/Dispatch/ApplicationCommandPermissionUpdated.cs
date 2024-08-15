using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class ApplicationCommandPermissionUpdated : IApplicationCommandPermissionUpdatedPayloadData
{
    [JsonIgnore, JsonExtend]
    public GuildApplicationCommandPermission GuildApplicationCommandPermission { get; set; } = null!;
}
