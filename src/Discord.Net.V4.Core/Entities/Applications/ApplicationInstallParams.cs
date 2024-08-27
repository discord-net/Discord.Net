using Discord.Models;
using Discord.Models.Json;

namespace Discord;

public readonly struct ApplicationInstallParams(
    IReadOnlyCollection<string> scopes,
    PermissionSet permissions
) :
    IModelConstructable<ApplicationInstallParams, IApplicationInstallParamsModel>,
    IEntityProperties<InstallParams>
{
    public IReadOnlyCollection<string> Scopes { get; } = scopes;
    public PermissionSet Permissions { get; } = permissions;

    public static ApplicationInstallParams Construct(IDiscordClient client, IApplicationInstallParamsModel model)
    {
        return new ApplicationInstallParams(model.Scopes.ToList().AsReadOnly(), model.Permissions);
    }

    public InstallParams ToApiModel(InstallParams? existing = default)
    {
        return new InstallParams()
        {
            Permissions = Permissions.ToString(),
            Scopes = Scopes.ToArray()
        };
    }
}