namespace Discord.Models;

public interface IApplicationInstallParamsModel
{
    string[] Scopes { get; }
    string Permissions { get; }
}