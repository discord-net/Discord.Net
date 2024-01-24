using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents properties used to modify current application's bot.
/// </summary>
public class ModifyApplicationProperties
{
    /// <summary>
    ///     Gets or sets the http interactions endpoint configured for the application.     
    /// </summary>
    public Optional<string> InteractionsEndpointUrl { get; set; }

    /// <summary>
    ///     Gets or sets the role connections verification endpoint configured for the application.
    /// </summary>
    public Optional<string> RoleConnectionsEndpointUrl { get; set; }

    /// <summary>
    ///     Gets or sets the description of the application.
    /// </summary>
    public Optional<string> Description { get; set; }

    /// <summary>
    ///     Gets or sets application's tags
    /// </summary>
    public Optional<string[]> Tags { get; set; }

    /// <summary>
    ///     Gets or sets the icon of the application.
    /// </summary>
    public Optional<Image?> Icon { get; set; }

    /// <summary>
    ///     Gets or sets the default rich presence invite cover image of the application.
    /// </summary>
    public Optional<Image?> CoverImage { get; set; }

    /// <summary>
    ///     Gets or set the default custom authorization URL for the app, if enabled.
    /// </summary>
    public Optional<string> CustomInstallUrl { get; set; }

    /// <summary>
    ///     Gets or sets settings for the app's default in-app authorization link, if enabled.
    /// </summary>
    public Optional<ApplicationInstallParams> InstallParams { get; set; }

    /// <summary>
    ///     Gets or sets app's public flags.
    /// </summary>
    /// <remarks>
    ///     Only <see cref="ApplicationFlags.GatewayGuildMembersLimited"/>, <see cref="ApplicationFlags.GatewayMessageContentLimited"/> and
    ///     <see cref="ApplicationFlags.GatewayPresenceLimited"/> flags can be updated.
    /// </remarks>
    public Optional<ApplicationFlags> Flags { get; set; }

    /// <summary>
    ///     Gets or sets application install params configured for integration install types.
    /// </summary>
    public Optional<Dictionary<ApplicationIntegrationType, ApplicationInstallParams>> IntegrationTypesConfig { get; set; }

}
