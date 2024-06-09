namespace Discord.Models;

public interface IApplicationRoleConnectionModel
{
    string? PlatformName { get; }
    string? PlatformUsername { get; }
    IDictionary<string, string> Metadata { get; }
}
