using Discord.Models.Json;

namespace Discord;

public class ModifyCurrentApplicationProperties : IEntityProperties<ModifyCurrentApplicationParams>
{
    public Optional<string> CustomInstallUrl { get; set; }
    public Optional<string> Description { get; set; }
    public Optional<string> RoleConnectionsVerificationUrl { get; set; }
    public Optional<ApplicationInstallParams> InstallParams { get; set; }

    public Optional<
        IDictionary<
            ApplicationIntegrationType,
            ApplicationIntegrationTypeConfiguration
        >
    > IntegrationTypesConfig { get; set; }
    
    public Optional<ModifyApplicationFlags> Flags { get; set; }
    
    public Optional<Image> Icon { get; set; }
    
    public Optional<Image> CoverImage { get; set; }

    public Optional<string> InteractionsEndpointUrl { get; set; }
    
    public Optional<string[]> Tags { get; set; }
    
    public ModifyCurrentApplicationParams ToApiModel(ModifyCurrentApplicationParams? existing = default)
    {
        return new ModifyCurrentApplicationParams()
        {
            Description = Description,
            Flags = Flags.MapToInt(),
            Tags = Tags,
            CoverImage = CoverImage.Map(v => v.ToImageData()),
            CustomInstallUrl = CustomInstallUrl,
            InteractionsEndpointUrl = InteractionsEndpointUrl,
            Icon = Icon.Map(v => v.ToImageData()),
            RoleConnectionsVerificationUrl = RoleConnectionsVerificationUrl,
            InstallParams = InstallParams.Map(v => v.ToApiModel()),
            IntegrationTypesConfig = IntegrationTypesConfig.Map(v =>
            {
                var value = new ApplicationIntegrationTypesConfig();

                if (v.TryGetValue(ApplicationIntegrationType.UserInstall, out var entry))
                    value.UserInstall = entry.ToApiModel();

                if (v.TryGetValue(ApplicationIntegrationType.GuildInstall, out entry))
                    value.GuildInstall = entry.ToApiModel();

                return value;
            })
        };
    }
}