namespace Discord;

public sealed class ModifyUserApplicationRoleConnection
{
    public Optional<string> PlatformName { get; set; }
    public Optional<string> PlatformUsername { get; set; }
    public Optional<Dictionary<string, object>> Metadata { get; set; } // TODO: proper class?
}
