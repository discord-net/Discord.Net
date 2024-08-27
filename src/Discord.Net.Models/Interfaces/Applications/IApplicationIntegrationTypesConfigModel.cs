namespace Discord.Models;

public interface IApplicationIntegrationTypesConfigModel
{
    IApplicationIntegrationTypeConfigurationModel? GuildInstall { get; }
    IApplicationIntegrationTypeConfigurationModel? UserInstall { get; }
}