using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class ApplicationIntegrationTypeConfiguration : IApplicationIntegrationTypeConfigurationModel
{
    [JsonPropertyName("oauth2_install_params")]
    public Optional<InstallParams> OAuth2InstallParams { get; set; }

    IApplicationInstallParamsModel? IApplicationIntegrationTypeConfigurationModel.OAuth2InstallParams 
        => ~OAuth2InstallParams;
}