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
}
