using Discord.Models;

namespace Discord;

public readonly struct ApplicationIntegrationTypeConfiguration(
    ApplicationInstallParams? oAuth2InstallParams
) :
    IModelConstructable<ApplicationIntegrationTypeConfiguration, IApplicationIntegrationTypeConfigurationModel>,
    IEntityProperties<Models.Json.ApplicationIntegrationTypeConfiguration>
{
    public ApplicationInstallParams? OAuth2InstallParams { get; } = oAuth2InstallParams;

    public static ApplicationIntegrationTypeConfiguration Construct(IDiscordClient client,
        IApplicationIntegrationTypeConfigurationModel model)
    {
        return new ApplicationIntegrationTypeConfiguration(
            model.OAuth2InstallParams is not null
                ? ApplicationInstallParams.Construct(client, model.OAuth2InstallParams)
                : null
        );
    }

    public Models.Json.ApplicationIntegrationTypeConfiguration ToApiModel(Models.Json.ApplicationIntegrationTypeConfiguration? existing = default)
    {
        return new Models.Json.ApplicationIntegrationTypeConfiguration()
        {
            OAuth2InstallParams = Optional.FromNullable(OAuth2InstallParams?.ToApiModel())
        };
    }
}