using Discord.Models.Json;

namespace Discord;

public sealed class ModifyApplicationCommandPermissionsProperties :
    IEntityProperties<ModifyApplicationCommandPermissionsParams>
{
    public IEnumerable<ApplicationCommandPermissions> Permissions { get; set; } = [];
    
    public ModifyApplicationCommandPermissionsParams ToApiModel(ModifyApplicationCommandPermissionsParams? existing = default)
    {
        return new ModifyApplicationCommandPermissionsParams()
        {
            Permissions = Permissions.Select(x => x.ToApiModel()).ToArray()
        };
    }
}