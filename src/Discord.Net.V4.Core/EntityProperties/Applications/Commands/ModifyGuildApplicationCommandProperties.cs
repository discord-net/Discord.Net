using System.Globalization;
using Discord.Models.Json;

namespace Discord;

public sealed class ModifyGuildApplicationCommandProperties :
    IEntityProperties<ModifyGuildApplicationCommandParams>
{
    public Optional<string> Name { get; set; }
    public Optional<Dictionary<CultureInfo, string>?> NameLocalizations { get; set; }
    public Optional<string> Description { get; set; }
    public Optional<Dictionary<CultureInfo, string>?> DescriptionLocalizations { get; set; }
    public Optional<IEnumerable<ApplicationCommandOption>> Options { get; set; }
    public Optional<PermissionSet> DefaultMemberPermissions { get; set; }
    public Optional<bool> IsNsfw { get; set; }
    
    public ModifyGuildApplicationCommandParams ToApiModel(ModifyGuildApplicationCommandParams? existing = default)
    {
        return new ModifyGuildApplicationCommandParams()
        {
            Name = Name,
            NameLocalizations = NameLocalizations.Map(v => v
                ?.ToDictionary(x => x.Key.Name, x => x.Value)
            ),
            Description = Description,
            DescriptionLocalizations = DescriptionLocalizations.Map(v => v
                ?.ToDictionary(x => x.Key.Name, x => x.Value)
            ),
            Options = Options.Map(v => v
                .Select(x => x.GetModel())
                .ToArray()
            ),
            DefaultMemberPermissions = DefaultMemberPermissions.Map<string?>(v => v.ToString()),
            IsNsfw = IsNsfw
        };
    }
}