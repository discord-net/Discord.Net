namespace Discord;

/// <summary>
///     Represents the style of a <see cref="MenuActionItemComponent"/>.
/// </summary>
public enum MenuActionItemStyle
{
    /// <summary>
    ///     The default style.
    /// </summary>
    Default = 1,

    /// <summary>
    ///     The danger style, used for destructive actions.
    /// </summary>
    Danger = 2,

    /// <summary>
    ///     The link style, used for action items that open a URL.
    /// </summary>
    /// <remarks>
    ///     Action items with this style must have a <see cref="MenuActionItemComponent.Url"/>.
    /// </remarks>
    Link = 3
}
