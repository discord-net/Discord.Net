namespace Discord.Models;

public interface IApplicationIntegrationTypeConfigurationModel
{
    IApplicationInstallParamsModel? OAuth2InstallParams { get; }
}