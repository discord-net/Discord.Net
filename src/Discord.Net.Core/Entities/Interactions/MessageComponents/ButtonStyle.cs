namespace Discord;

/// <summary>
///     Represents different styles to use with buttons. You can see an example of the different styles at <see href="https://discord.com/developers/docs/interactions/message-components#buttons-button-styles"/>
/// </summary>
public enum ButtonStyle
{
    /// <summary>
    ///     A Blurple button.
    /// </summary>
    Primary = 1,

    /// <summary>
    ///     A Grey (or gray) button.
    /// </summary>
    Secondary = 2,

    /// <summary>
    ///     A Green button.
    /// </summary>
    Success = 3,

    /// <summary>
    ///     A Red button.
    /// </summary>
    Danger = 4,

    /// <summary>
    ///     A <see cref="Secondary"/> button with a little popup box indicating that this button is a link.
    /// </summary>
    Link = 5,

    /// <summary>
    ///     A gradient button, opens a product's details modal.
    /// </summary>
    Premium = 6,
}
