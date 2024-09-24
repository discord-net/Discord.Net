using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationIntegrationTypesConfig : IApplicationIntegrationTypesConfigModel
{
    [JsonPropertyName("0")]
    public Optional<ApplicationIntegrationTypeConfiguration> GuildInstall { get; set; }

    [JsonPropertyName("1")]
    public Optional<ApplicationIntegrationTypeConfiguration> UserInstall { get; set; }

    IApplicationIntegrationTypeConfigurationModel? IApplicationIntegrationTypesConfigModel.UserInstall 
        => ~UserInstall;

    IApplicationIntegrationTypeConfigurationModel? IApplicationIntegrationTypesConfigModel.GuildInstall 
        => ~GuildInstall;
}